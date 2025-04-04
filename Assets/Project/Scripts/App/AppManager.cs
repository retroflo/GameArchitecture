using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace LWFlo
{
    /// <summary>
    /// AppManager handles the application state transitions and provides
    /// events for state changes.
    /// </summary>
    public class AppManager : IDisposable
    {
        
        private readonly GameManager _gameManager;
        private readonly AppSceneManager _appSceneManager;
        
        private readonly CancellationTokenSource _appCts;
        
        [Inject]
        public AppManager(GameManager gameManager, AppSceneManager appSceneManager)
        {
            _gameManager = gameManager;
            _appSceneManager = appSceneManager;

            _appCts = new CancellationTokenSource();
        }
        
        public async void Initialize()
        {
            try
            {
                await RunInitialize(_appCts.Token);
                await RunLoading(_appCts.Token);
                await RunPlaying(_appCts.Token);
            }
            catch (Exception e)
            {
                await RunError(_appCts.Token, e);
                throw;
            }
            
            await RunQuiting(_appCts.Token);
        }

        private async UniTask RunInitialize(CancellationToken cancellationToken)
        {
            Debug.Log($"[{nameof(AppManager)}] : App initializing");
            
            _appSceneManager.Initialize();
            await _gameManager.Initialize(cancellationToken);
            
            Debug.Log($"[{nameof(AppManager)}] : App was successfully initialized");
        }
        
        private async UniTask RunLoading(CancellationToken cancellationToken)
        {
            Debug.Log($"[{nameof(AppManager)}] : App loading");
            
            
            
            Debug.Log($"[{nameof(AppManager)}] : App was successfully loaded");
        }
        
        private async UniTask RunPlaying(CancellationToken cancellationToken)
        {
            Debug.Log($"[{nameof(AppManager)}] : App playing");
            await _gameManager.StartWithGameState(cancellationToken);
        }

        private async UniTask RunError(CancellationToken cancellationToken, Exception exception)
        {
            Debug.Log($"[{nameof(AppManager)}] : App error");
            Debug.LogError($"[{nameof(AppManager)}] : Error initializing: {exception}");
            
#if UNITY_EDITOR
            if (Application.isEditor)
                UnityEditor.EditorApplication.ExitPlaymode();
#endif
                
            Application.Quit(-1);
        }
        
        private async UniTask RunQuiting(CancellationToken cancellationToken)
        {
            Debug.Log($"[{nameof(AppManager)}] : App quiting");
        }

        public void Dispose()
        {
            _appCts?.Dispose();
        }
    }
}
