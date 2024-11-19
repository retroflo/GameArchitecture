using System.Threading;
using Cysharp.Threading.Tasks;

namespace LWFlo
{
    public interface IGameStateWithContext< in TContext >
        : IGameStateBase
        where TContext : class // Ensure context is nullable
    {
        UniTask Run(TContext context, CancellationToken cancellationToken);
    }
    
    public interface IGameStateBase
    {
        
    }
}