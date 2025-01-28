using System.Threading;
using Cysharp.Threading.Tasks;

namespace LWFlo.Game
{
    public interface IGameStateWithContext< in TContext >
        : IGameState
        where TContext : class // Ensure context is nullable
    {
        UniTask Run(TContext context, CancellationToken cancellationToken);
    }
    
    public interface IGameState
    {
        public bool isRunning { get; }
        public bool isSuspended { get; }

        void Resume();
        void Suspend();
    }
}