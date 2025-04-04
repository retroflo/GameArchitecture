using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LWFlo
{
    public class MenuGameState : IGameState
    {
        private readonly GameManager _gameManager;
        private readonly AppSceneManager _appSceneManager;
        private readonly MenuScreen _uiMenu;

        private MenuScreen _uiMenuScreen;
        private UniTaskCompletionSource<bool> _buttonClickedTaskSource;
        private bool _shouldExit;
        
        [Inject]
        public MenuGameState(GameManager gameManager, AppSceneManager appSceneManager, MenuScreen uiMenu)
        {
            _gameManager = gameManager;
            _appSceneManager = appSceneManager;
            _uiMenu = uiMenu;
        }
        
        public async UniTask OnInitialize(CancellationToken cancellationToken)
        {
            var appView = _appSceneManager.GetUiRoot();
            var menuUIRoot = appView.GetUIMenuRoot();
            
            _uiMenuScreen = Object.Instantiate(_uiMenu, menuUIRoot.transform);
            _uiMenuScreen.name = Texts.UI_MENU;
            
            _uiMenuScreen.OnButtonClicked += HandleButtonClick;
        }
        
        public async UniTask OnRun(CancellationToken cancellationToken)
        {
            _uiMenuScreen.RegisterAllButtons();
            
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

                if(race.Result == 1)
                    continue;
            }
        }

        private void HandleButtonClick(string buttonName)
        {
            switch (buttonName)
            {
                case Texts.PLAY_BUTTON:
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
                Debug.Log($"[{nameof(MenuGameState)}] Button click wait cancelled");
            }
        }

        private void Dispose()
        {
            _uiMenuScreen.OnButtonClicked -= HandleButtonClick;
            _uiMenuScreen.DisposeScreen();

            _gameManager.SwitchToGameplayState();
        }
    }
}
