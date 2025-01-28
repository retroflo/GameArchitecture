using System.Threading;
using Cysharp.Threading.Tasks;

namespace LWFlo.Game
{
    public interface IGameManager
    {
        UniTask StartWithState<TState, TContext>(TContext ctx, CancellationToken cancellationToken)
            where TContext : class
            where TState : GameStateBase<TContext>;
        
        UniTask EnqueuePushState<TState, TContext>(TContext ctx, CancellationToken cancellationToken)
            where TContext : class
            where TState : GameStateBase<TContext>;

        void EnqueueSwitchState<TState, TContext>(TContext ctx)
            where TContext : class
            where TState : GameStateBase<TContext>;

        string GetBreadcrumbs();
        string GetCurrentState();
    }
}