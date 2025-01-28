using System.Threading;
using Cysharp.Threading.Tasks;

namespace LWFlo.Game
{
    public abstract class GameStateBase<TContext> :
        IGameStateWithContext<TContext> where TContext : class
    {
        public bool isRunning => _isRunning;
        public bool isSuspended => _isSuspended;

        private TContext _context;
        private bool _isRunning;
        private bool _isSuspended;
        
        public UniTask Run(TContext context, CancellationToken cancellationToken)
        {
            _isRunning = true;
            _context = context;
            
            return OnRun(cancellationToken);
        }
        
        protected abstract UniTask OnRun(CancellationToken cancellationToken);
        
        public void Suspend()
        {
            _isSuspended = true;
            OnSuspend();
        }

        public void Resume()
        {
            _isSuspended = false;
            OnResume();
        }
        
        protected abstract void OnSuspend();
        protected abstract void OnResume();
    }
}