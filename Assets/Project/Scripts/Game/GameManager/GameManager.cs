using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LWFlo.Project.Scripts.Tools;
using VContainer;

namespace LWFlo
{
    public class GameManager : IGameManager
    {
        private readonly IObjectResolver _container;
        
        [Inject]
        public GameManager(IObjectResolver container)
        {
            _container = container;
        }
        public async UniTask StartWithState<TState, TContext>(TContext ctx, CancellationToken cancellationToken)
            where TContext : class
            where TState : GameStateBase<TContext>
        {
            var stateInstance = _container.Resolve<TState>();
            if (stateInstance.IsNull())
                throw new InvalidOperationException("Failed to create state instance");
            
            if (stateInstance is IGameStateWithContext<TContext> stateWithContext)
            {
                try
                {
                    await stateWithContext.Run(ctx, cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            
        }
    }
}
