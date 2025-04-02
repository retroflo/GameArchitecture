using UnityEngine;
using VContainer.Unity;

namespace LWFlo
{
    [CreateAssetMenu(menuName = "LWFlo/Configuration/Child Scope Configuration")]
    public class ChildScopeConfiguration : ScriptableObject
    {
        public LifetimeScope[] scopes;
    }
}