using System;
using VContainer;
using Object = UnityEngine.Object;

namespace LWFlo
{
    public class AppSceneManager
    {
        private readonly AppView _uiRoot;
        
        private AppView _instantiatedAppView;
        
        [Inject]
        public AppSceneManager(AppView uiRoot)
        {
            _uiRoot = uiRoot;
        }

        public void Initialize()
        {
            if (_uiRoot.IsNull() == true)
                throw new Exception($"{nameof(AppSceneManager)} : Couldn't load app view");
            
            if (_instantiatedAppView.IsNotNull() == true)
                return;
            
            _instantiatedAppView = Object.Instantiate(_uiRoot);
            _instantiatedAppView.name = Texts.UI_ROOT;
        }

        public AppView GetUiRoot()
        {
            return _instantiatedAppView;
        }
    }
}