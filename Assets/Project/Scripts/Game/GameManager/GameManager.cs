using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LWFlo.Tools;
using MessagePipe;
using Project.Scripts.Messages.Requests.Events;
using UnityEngine;
using VContainer;

namespace LWFlo.Game
{
    public class GameManager : IGameManager, IDisposable
    {
        private class StateHandle
        {
            public IGameState state;
            public bool isRootState;
            
            public Func<UniTask> activation;
            public UniTaskCompletionSource completion;
            public CancellationTokenSource cancellation;
        }
        
        private readonly IObjectResolver _container;
        private readonly CancellationTokenSource _runCancellation;
        private readonly Stack<StateHandle> _stateStack;
        private readonly List<string> _breadcrumbs;
        private readonly IPublisher<GameStateChangedMessage> _stateChangePublisher;

        private bool _wasDisposed;
        private StateHandle _queuedRequest;

        
        [Inject]
        public GameManager(IObjectResolver container, IPublisher<GameStateChangedMessage> stateChangePublisher)
        {
            _container = container;
            _stateChangePublisher = stateChangePublisher;

            _queuedRequest = null;
            _breadcrumbs = new List<string>();
            _runCancellation = new CancellationTokenSource();
            _stateStack = new Stack<StateHandle>();
        }
        public async UniTask StartWithState<TState, TContext>(TContext ctx, CancellationToken cancellationToken)
            where TContext : class
            where TState : GameStateBase<TContext>
        {
            var stateInstance = _container.Resolve<TState>();
            if (stateInstance.IsNull())
                throw new InvalidOperationException("Failed to create state instance");
            
            _queuedRequest = null;
            var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(_runCancellation.Token, cancellationToken).Token;
            
            var first = CreateStateHandle<TState, TContext>( ctx, true, linkedCancellationToken );
            _stateStack.Push(first);
            
            while (_stateStack.Count > 0 && linkedCancellationToken.IsCancellationRequested == false)
            {
                var top = _stateStack.Peek();
                
                if (top.state.isRunning == false)
                    _breadcrumbs.Add(top.state.GetType().Name);
                
                _stateChangePublisher.Publish(new GameStateChangedMessage
                {
                    currentState = top.state.GetType().Name
                });
                
                if (top.state.isRunning == false)
                {
                    Debug.Log($"[GameManager] Activating State ({top.state.GetType().Name})");
                    top.activation
                        .Invoke()
                        .Forget();
                }

                if (top.state.isSuspended == true)
                {
                    Debug.Log($"[GameManager] Resuming State ({top.state.GetType().Name})");
                    top.state.Resume();
                }

                try
                {
                    await UniTask.WhenAny(
                        top.completion.Task,
                        UniTask.WaitUntil(() => _queuedRequest != null, cancellationToken: linkedCancellationToken)
                    );
                }
                catch (OperationCanceledException)
                {
                    // In case of a cancellation caught from the tasks waiting for the current state to complete, or for a new state to be queued
                    // IF the cancellation originates from the externally provided cancellation source (e.g. AppManager -> RequestCancelGameManager)
                    // This is a valid cancellation scenario, we do not need to throw in this case but instead handle it by gracefully exiting the state run loop
                    // In any other case of a cancellation, it is fatal and we want to rethrow
                    if (cancellationToken.IsCancellationRequested == true)
                        _queuedRequest = null;
                    else
                        throw;
                }

                // Interrupted by queued operation
                if (_queuedRequest != null)
                {
                    var req = _queuedRequest;
                    _queuedRequest = null;

                    // If queued state cancellation was requested before we processed it, we can just ignore it and return to normal operation
                    if (req.cancellation.IsCancellationRequested == false)
                    {
                        // New root state, terminate all existing stack first
                        if (req.isRootState == true)
                        {
                            var pendingCompletion = new List<UniTask>();
                            while (_stateStack.Count > 0)
                            {
                                var innerTop = _stateStack.Pop();
                                Debug.Log($"[GameManager] Cancelling State ({innerTop.state.GetType().Name})");
                                
                                innerTop.cancellation?.Cancel();
                                pendingCompletion.Add(innerTop.completion.Task);
                            }

                            await UniTask.WhenAll(pendingCompletion);
                            
                            _breadcrumbs.Clear();
                        }
                        else
                        {
                            var innerTop = _stateStack.Peek();
                            Debug.Log($"[GameManager] Suspending State ({innerTop.state.GetType().Name})");
                            
                            innerTop.state.Suspend();
                        }

                        // Push new handle on top, will be activated on next iteration
                        _stateStack.Push(req);
                    }
                }
                // Interrupted by completion, pop completed state
                else
                {
                    _stateStack.Pop();
                    _breadcrumbs.RemoveAt(_breadcrumbs.Count - 1);
                }
            }

            _queuedRequest = null;
            Debug.Log($"[GameManager] Completed run with initial state ({typeof(TState).Name}, {typeof(TContext).Name})");
        }
        
        public void EnqueueSwitchState<TState, TContext>( TContext ctx) 
            where TContext : class 
            where TState : GameStateBase<TContext>
        {
            if (_wasDisposed == true)
                return;
        
            Debug.Log($"[GameManager] Enqueue switch state ({typeof(TState).Name}, {typeof(TContext).Name})");
            _queuedRequest = CreateStateHandle<TState, TContext>(ctx, true, CancellationToken.None);
        }
        
        private StateHandle CreateStateHandle<TState, TContext>(TContext ctx, bool isRootState, CancellationToken cancellationToken)
            where TContext : class
            where TState : GameStateBase<TContext>
        {
            var state = CreateInstance<TState, TContext>();
            var stateCancellation = new CancellationTokenSource();
            var stateCompletion = new UniTaskCompletionSource();
            
            var activation = new Func<UniTask>(() =>
            {
                var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, stateCancellation.Token, _runCancellation.Token);
                return RunState(state, ctx, stateCompletion, linkedCancellation.Token);
            });
                
            var handle = new StateHandle
            {
                state = state,
                isRootState = isRootState,
                activation = activation,
                cancellation = stateCancellation,
                completion = stateCompletion,
            };
            
            return handle;
        }
        
        private TState CreateInstance<TState, TContext>()
            where TContext : class 
            where TState : GameStateBase<TContext>
        {
            var instance = _container.Resolve<TState>();

            if (instance == null)
                throw new InvalidOperationException("Failed to create state instance");

            return instance;
        }
        
        public UniTask EnqueuePushState<TState, TContext>( TContext ctx, CancellationToken cancellationToken )
            where TContext : class 
            where TState : GameStateBase<TContext>
        {
            if (_wasDisposed == true)
                return UniTask.FromCanceled(cancellationToken);

            if (IsTopPendingCompletion() == true )
                return UniTask.
                    WaitUntil(() => IsTopPendingCompletion() == false, cancellationToken: cancellationToken)
                    .ContinueWith(() => EnqueuePushState<TState, TContext>(ctx, cancellationToken));
            
            Debug.Log($"[GameManager] Enqueue push state ({typeof(TState).Name}, {typeof(TContext).Name})");
            _queuedRequest = CreateStateHandle<TState, TContext>(ctx, false, cancellationToken);

            return _queuedRequest.completion.Task;
        }
        
        private async UniTask RunState<TContext>(IGameState state, TContext ctx, UniTaskCompletionSource tcs, CancellationToken cancellationToken)
            where TContext : class
        {
            if (state is IGameStateWithContext<TContext> stateWithContext)
            {
                Debug.Log($"[GameManager] Run game state - {stateWithContext.GetType().Name}, with context - {ctx}");
                
                try
                {
                    var isCancelled = await stateWithContext
                        .Run(ctx, cancellationToken)
                        .SuppressCancellationThrow();

                    if (isCancelled)
                        tcs.TrySetCanceled();
                    else
                        tcs.TrySetResult();
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }
            else
            {
                throw new InvalidOperationException($"Cannot enter state since current state is not IStateEnter");
            }
        }
        
        public string GetCurrentState()
        {
            if (_stateStack.Count <= 0)
                return string.Empty;

            var top = _stateStack.Peek();
            return top.state.GetType().Name;
        }
        
        public string GetBreadcrumbs()
        {
            return string.Join(" > ", _breadcrumbs);
        }
        
        private bool IsTopPendingCompletion()
        {
            if (_stateStack.Count <= 0)
                return false;
            
            var top = _stateStack.Peek();
            var status = top.completion.UnsafeGetStatus();

            return status == UniTaskStatus.Succeeded;
        }

        public void Dispose()
        {
            if(_wasDisposed == true)
                return;
            
            if (_queuedRequest != null)
            {
                var stateName = _queuedRequest.state.GetType().Name;
                
                Debug.Log($"[GameManager] Cancelling Queued State Change ({stateName})");
                _queuedRequest.completion.TrySetCanceled();
            }
            
            _runCancellation?.Cancel();
            _wasDisposed = true;
        }
    }
}
