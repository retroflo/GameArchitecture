using System;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using LWFlo.Project.Scripts.Constants;
using LWFlo.Project.Scripts.Messages.Requests.Game;
using LWFlo.States;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace LWFlo
{
    public class AppManager
    {
        private readonly CancellationTokenSource _disposeCancellation;
        private readonly LifetimeScope _currentScope;
        
        private readonly IRequestHandler<CreateScopeRequest, CreateScopeResponse> _scopeCreator;
        private readonly IRequestHandler<FetchScopeRequest, FetchScopeResponse> _scopeRetriever;
        
        private Exception _exception;
        
        [Inject]
        public AppManager(LifetimeScope currentScope,
            IRequestHandler<CreateScopeRequest, CreateScopeResponse> scopeCreator,
            IRequestHandler<FetchScopeRequest, FetchScopeResponse> scopeRetriever)
        {
            _currentScope = currentScope;
            _scopeCreator = scopeCreator;
            _scopeRetriever = scopeRetriever;
        }

        public void Start()
        {
            RunInitial();
        }

        private async void RunInitial()
        {
            try
            {
                await RunInitialState(CancellationToken.None);
                await RunLoadingState(CancellationToken.None);
                await RunPlayingState(CancellationToken.None);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private async UniTask RunInitialState(CancellationToken cancellationToken)
        {
            // Analytics and social
        }
        
        private async UniTask RunLoadingState(CancellationToken cancellationToken)
        {
            await UniTask.NextFrame(cancellationToken); // Must wait at least one frame before creating child scope

            var gameScope = default(GameScope);

            try
            {
                // Create Scope
                var scopeResult = _scopeCreator.Invoke(new CreateScopeRequest
                    { childName = ScopeNames.GAME_SCOPE, parentScope = _currentScope });

                gameScope = (GameScope)scopeResult.childScope;
            }
            catch (TargetInvocationException ex)
            {
                // Unwrap exception for better visibility of errors during construction
                if (ex.InnerException == null)
                    throw;

                throw ex.InnerException;
            }
            
            await UniTask.NextFrame(cancellationToken); // Must wait at least one frame before creating child scope
        }
        
        private async UniTask RunPlayingState(CancellationToken cancellationToken)
        {
            var gameScope = _scopeRetriever.Invoke(new FetchScopeRequest{ scopeName = ScopeNames.GAME_SCOPE });
            var gameManager = gameScope.scope.Container.Resolve<IGameManager>();

            var context = new GameStateGameplayOrResume.Context { };
            await gameManager.StartWithState<GameStateGameplayOrResume, GameStateGameplayOrResume.Context>(context, cancellationToken);
        }
    }
}