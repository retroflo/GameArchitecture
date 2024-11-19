using RetroCloud.BioVirus.App;
using UnityEngine;

namespace LWFlo
{
    [CreateAssetMenu(menuName = "LWFlo/Configuration/App Scope Configuration")]
    public class AppScopeConfiguration : ScriptableObject
    {
        public ChildScopeConfiguration childScopeConfiguration;
    }
}