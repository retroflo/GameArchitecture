using UnityEngine;
using MessagePipe;
using VContainer;

namespace LWFlo
{
    /// <summary>
    /// AppManager handles the application state transitions and provides
    /// events for state changes.
    /// </summary>
    public class AppManager
    {
        
        private readonly GameManager _gameManager;
        private AppState _currentState = AppState.Initializing;
        
        [Inject]
        public AppManager(IRequestHandler<CreateScopeRequest, CreateScopeResponse> scopeCreator, GameManager gameManager)
        {
            _gameManager = gameManager;
        }
        
        public void Initialize()
        {
            Debug.Log($"[{nameof(AppManager)}] Initializing");
            SetState(AppState.Initializing);
            
            _gameManager.Initialize();
        }
        
        public void SetLoading()
        {
            SetState(AppState.Loading);
        }
        
        public void SetPlaying()
        {
            SetState(AppState.Playing);
        }
        
        public void SetPaused()
        {
            SetState(AppState.Paused);
        }
        
        public void SetError(string errorMessage = null)
        {
            if (errorMessage != null)
                Debug.LogError($"[{nameof(AppManager)}] Error: {errorMessage}");
            
            SetState(AppState.Error);
        }
        
        public void SetQuitting()
        {
            SetState(AppState.Quitting);
        }
        
        private void SetState(AppState newState)
        {
            if (_currentState == newState)
                return;
            
            var previousState = _currentState;
            _currentState = newState;
            
            Debug.Log($"[{nameof(AppManager)}] State changed from {previousState} to {_currentState}");
        }
    }
}
