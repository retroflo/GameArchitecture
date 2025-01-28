using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;

namespace LWFlo.Game
{
    public class GameStateMenu : GameStateBase<GameStateMenu.Context>
    {
        public class Context { }
        
        [Inject]
        public GameStateMenu()
        {
            
        }

        protected override async UniTask OnRun(CancellationToken cancellationToken)
        {
            
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