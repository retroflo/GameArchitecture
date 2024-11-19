using UnityEngine;
using VContainer.Unity;

namespace RetroCloud.BioVirus.App
{
    [CreateAssetMenu(menuName = "LWFlo/Configuration/Child Scope Configuration")]
    public class ChildScopeConfiguration : ScriptableObject
    {
        public LifetimeScope[] scopes;
    }
}