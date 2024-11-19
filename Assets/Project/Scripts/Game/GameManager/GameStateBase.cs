using System.Threading;
using Cysharp.Threading.Tasks;

namespace LWFlo
{
    public abstract class GameStateBase<TContext> :
        IGameStateWithContext<TContext> where TContext : class
    {
        private TContext _context;
        private bool _isRunning;
        
        public UniTask Run(TContext context, CancellationToken cancellationToken)
        {
            _isRunning = true;
            _context = context;
            
            return OnRun(cancellationToken);
        }
        
        protected abstract UniTask OnRun(CancellationToken cancellationToken);
    }
}