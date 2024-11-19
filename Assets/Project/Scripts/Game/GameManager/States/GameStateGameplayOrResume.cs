using System.Threading;
using Cysharp.Threading.Tasks;

namespace LWFlo.States
{
    public class GameStateGameplayOrResume : GameStateBase<GameStateGameplayOrResume.Context>
    {
        public class Context {}
        protected override UniTask OnRun(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}