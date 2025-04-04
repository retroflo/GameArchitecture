using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LWFlo
{
    /// <summary>
    /// GameManager handles switching between game states (menu and gameplay)
    /// and manages the game's overall state.
    /// </summary>
    public class GameManager
    {
        private readonly IRequestHandler<CreateScopeRequest, CreateScopeResponse> _scopeCreator;
        private readonly IRequestHandler<DisposeScopeRequest, DisposeScopeResponse> _disposableScope;
        
        private readonly List<IGameState> _gameStates;
        private readonly LifetimeScope _currentScope;
        
        private ScopeName _currentScopeName;
        private IGameState _currentState;

        [Inject]
        public GameManager(IRequestHandler<CreateScopeRequest, CreateScopeResponse> scopeCreator, LifetimeScope currentScope, 
            IRequestHandler<DisposeScopeRequest, DisposeScopeResponse> disposableScope)
        {
            _scopeCreator = scopeCreator;
            _currentScope = currentScope;
            _disposableScope = disposableScope;
            _gameStates = new List<IGameState>();
        }

        public async UniTask Initialize(CancellationToken cancellationToken)
        {
            try
            {
                _gameStates.Clear();
                _currentState = null;
                
                _scopeCreator.Invoke(new CreateScopeRequest
                    { childName = ScopeName.MenuScope.ToString(), parentScope = _currentScope });
                
                var menuGameState = _currentScope.Container.Resolve<MenuGameState>();
                PushState(menuGameState);
                
                Debug.Log($"[{nameof(GameManager)}] {ScopeName.MenuScope} created during initialization");
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(GameManager)}] Error during initialization: {e}");
                throw;
            }
        }

        public async UniTask StartWithGameState(CancellationToken cancellationToken)
        {
            try
            {
                while (_gameStates.Count > 0 && cancellationToken.IsCancellationRequested == false)
                {
                    await ProcessNextState(cancellationToken);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(GameManager)}] Error processing game states: {e}");
                throw;
            }
        }

        public void SwitchToGameplayState()
        {
            _disposableScope.Invoke(new DisposeScopeRequest
                { childName = ScopeName.MenuScope.ToString(), });
            
            var gameplayState = _currentScope.Container.Resolve<GameplayState>();
            PushState(gameplayState);
            
            _scopeCreator.Invoke(new CreateScopeRequest
                { childName = ScopeName.GameplayScope.ToString(), parentScope = _currentScope });
        }

        public void SwitchToMenuState()
        {
            _disposableScope.Invoke(new DisposeScopeRequest
                { childName = ScopeName.GameplayScope.ToString(), });
            
            var menuState = _currentScope.Container.Resolve<MenuGameState>();
            PushState(menuState);
            
            _scopeCreator.Invoke(new CreateScopeRequest
                { childName = ScopeName.MenuScope.ToString(), parentScope = _currentScope });
        }

        public void PushState(IGameState state)
        {
            _gameStates.Add(state);
        }

        private async UniTask ProcessNextState(CancellationToken cancellationToken)
        {
            if (_gameStates.Count == 0)
            {
                Debug.Log($"[{nameof(GameManager)}] No more states to process");
                return;
            }
    
            _currentState = _gameStates.Last();
            await _currentState.OnInitialize(cancellationToken);
            Debug.Log($"[{nameof(GameManager)}] Processing state: {_currentState.GetType().Name}");
            
            var raceCts = new CancellationTokenSource();
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(raceCts.Token, cancellationToken);

            var race = await UniTask.WhenAny(
                    OnStateChanged(linkedCts.Token),
                    OnStateDone(linkedCts.Token)
                )
                .SuppressCancellationThrow();
            raceCts.Cancel();
            
            // On State Done
            if(race.Result == 1)
                _gameStates.Remove(_currentState);
        }

        private async UniTask OnStateChanged(CancellationToken cancellationToken)
        {
            var currentStateQueued = _gameStates.Count;
            await UniTask.WaitUntil(() => currentStateQueued != _gameStates.Count,
                cancellationToken: cancellationToken);
        }

        private async UniTask OnStateDone(CancellationToken cancellationToken)
        {
            await _currentState.OnRun(cancellationToken);
        }
    }
}
