using UnityEngine;

namespace LWFlo
{
    [CreateAssetMenu(menuName = "LWFlo/Configuration/App Scope Configuration")]
    public class AppScopeConfiguration : ScriptableObject
    {
        public ChildScopeConfiguration childScopeConfiguration;
        public AppView uiRoot;
        public MenuScreen uiMenu;
        public GameplayScreen uiGameplay;
    }
}