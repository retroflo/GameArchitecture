using UnityEngine;

namespace LWFlo.App
{
    [CreateAssetMenu(menuName = "LWFlo/Configuration/App Scope Configuration")]
    public class AppScopeConfiguration : ScriptableObject
    {
        public ChildScopeConfiguration childScopeConfiguration;
    }
}