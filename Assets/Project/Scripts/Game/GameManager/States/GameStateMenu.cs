using System.Threading;
using Cysharp.Threading.Tasks;
using LWFlo.Constants;
using LWFlo.Messages;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace LWFlo.Game
{
    public class GameStateMenu : GameStateBase<GameStateMenu.Context>
    {
        public class Context { }
        
        private readonly LifetimeScope _currentScope;
        private readonly IRequestHandler<CreateScopeRequest, CreateScopeResponse> _scopeCreator;
        private readonly ISceneSystem _sceneSystem;
        
        
        [Inject]
        public GameStateMenu(LifetimeScope currentScope, 
            IRequestHandler<CreateScopeRequest, CreateScopeResponse> scopeCreator, 
            ISceneSystem sceneSystem)
        {
            _currentScope = currentScope;
            _scopeCreator = scopeCreator;
            _sceneSystem = sceneSystem;
        }

        protected override async UniTask OnRun(CancellationToken cancellationToken)
        {
            // Creating a new scope
            var scopeResult = _scopeCreator.Invoke(new CreateScopeRequest
            {
                childName = ScopeNames.MENU_SCOPE,
                parentScope = _currentScope,
            });

            var scope = scopeResult.childScope;
            var scene = _sceneSystem.GetScene();
        }

        protected override void OnSuspend()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnResume()
        {
            throw new System.NotImplementedException();
        }
    }
}