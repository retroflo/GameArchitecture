using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;

namespace LWFlo.Game
{
    public class GameStateGameplayOrMenu : GameStateBase<GameStateGameplayOrMenu.Context>
    {
        private readonly ILocalDataStorage _localDataStorage;
        private readonly IGameManager _gameManager;
        
        [Inject]
        public GameStateGameplayOrMenu(ILocalDataStorage localDataStorage, IGameManager gameManager)
        {
            _localDataStorage = localDataStorage;
            _gameManager = gameManager;
        }
        public class Context {}
        protected override async UniTask OnRun(CancellationToken cancellationToken)
        {
            var doesDataExists = _localDataStorage.Has();
            if (doesDataExists == true)
            {
               _localDataStorage.Clear();
            }
            else
            {
                var menuContext = new GameStateMenu.Context();
                _gameManager.EnqueueSwitchState<GameStateMenu, GameStateMenu.Context>(menuContext);
            }
        }

        protected override void OnSuspend()
        {
            throw new NotImplementedException();
        }

        protected override void OnResume()
        {
            throw new NotImplementedException();
        }
    }
}