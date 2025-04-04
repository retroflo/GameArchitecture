using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LWFlo
{
    public class GameplayState : IGameState
    {
        private readonly GameManager _gameManager;
        private readonly AppSceneManager _appSceneManager;
        private readonly GameplayScreen _uiGameplay;

        private GameplayScreen _uiGameplayScreen;
        private UniTaskCompletionSource<bool> _buttonClickedTaskSource;
        private bool _shouldExit;
        
        [Inject]
        public GameplayState(GameManager gameManager, AppSceneManager appSceneManager, GameplayScreen uiGameplay)
        {
            _gameManager = gameManager;
            _appSceneManager = appSceneManager;
            _uiGameplay = uiGameplay;
        }
        
        public async UniTask OnInitialize(CancellationToken cancellationToken)
        {
            var appView = _appSceneManager.GetUiRoot();
            var uiRoot = appView.GetUIGameplayRoot();
            
            _uiGameplayScreen = Object.Instantiate(_uiGameplay, uiRoot.transform);
            _uiGameplayScreen.name = Texts.UI_GAMEPLAY;
            
            _uiGameplayScreen.OnButtonClicked += HandleButtonClick;
        }
        
        public async UniTask OnRun(CancellationToken cancellationToken)
        {
            _uiGameplayScreen.RegisterAllButtons();
            
            while (cancellationToken.IsCancellationRequested == false)
            {
                var raceCts = new CancellationTokenSource();
                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(raceCts.Token, cancellationToken);
                
                _buttonClickedTaskSource = new UniTaskCompletionSource<bool>();
                
                var race = await UniTask.WhenAny(
                        OnPlayerInput(linkedCts.Token)
                    )
                    .SuppressCancellationThrow();
                raceCts.Cancel();
                
                if (race.Result == 0)
                    if (_shouldExit == true)
                    {
                        Dispose();
                        return;
                    }
            }   
        }

        private void HandleButtonClick(string buttonName)
        {
            Debug.Log($"[{nameof(GameplayState)}] Processing button click: {buttonName}");
            
            switch (buttonName)
            {
                case Texts.QUIT_BUTTON:
                    _shouldExit = true;
                    break;
            }
            
            _buttonClickedTaskSource?.TrySetResult(true);
        }

        private async UniTask OnPlayerInput(CancellationToken cancellationToken)
        {
            try
            {
                await _buttonClickedTaskSource.Task.AttachExternalCancellation(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"[{nameof(GameplayState)}] Button click wait cancelled");
            }
        }

        private void Dispose()
        {
            _uiGameplayScreen.OnButtonClicked -= HandleButtonClick;
            _uiGameplayScreen.DisposeScreen();
            
            _gameManager.SwitchToMenuState();
        }
    }
}
