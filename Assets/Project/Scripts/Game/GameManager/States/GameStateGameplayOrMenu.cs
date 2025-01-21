using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;

namespace LWFlo.Game
{
    public class GameStateGameplayOrMenu : GameStateBase<GameStateGameplayOrMenu.Context>
    {
        private readonly ILocalDataStorage _localDataStorage;
        
        [Inject]
        public GameStateGameplayOrMenu(ILocalDataStorage localDataStorage)
        {
            _localDataStorage = localDataStorage;
        }
        public class Context {}
        protected override async UniTask OnRun(CancellationToken cancellationToken)
        {
            var doesDataExists = _localDataStorage.Has();
        }
    }
}