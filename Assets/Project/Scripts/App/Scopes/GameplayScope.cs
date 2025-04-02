using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LWFlo
{
    public class GameplayScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[{nameof(GameplayScope)}] Configured");
        }

        protected override void OnDestroy()
        {
            Debug.Log($"[{nameof(GameplayScope)}] Destroyed");
            base.OnDestroy();
        }
    }
}
