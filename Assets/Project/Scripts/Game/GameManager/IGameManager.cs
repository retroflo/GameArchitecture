using System.Threading;
using Cysharp.Threading.Tasks;

namespace LWFlo
{
    public interface IGameManager
    {
        UniTask StartWithState<TState, TContext>(TContext ctx, CancellationToken cancellationToken)
            where TContext : class
            where TState : GameStateBase<TContext>;
    }
}