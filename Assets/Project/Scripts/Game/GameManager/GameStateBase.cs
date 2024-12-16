using System.Threading;
using Cysharp.Threading.Tasks;

namespace LWFlo.Game
{
    public abstract class GameStateBase<TContext> :
        IGameStateWithContext<TContext> where TContext : class
    {
        public bool isRunning => _isRunning;
        
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