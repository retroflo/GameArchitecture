using JetBrains.Annotations;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LWFlo
{
    [UsedImplicitly]
    public class MenuScope : LifetimeScope
    {        
        protected override void Configure(IContainerBuilder builder)
        {
            // Register message handlers
            var options = builder.RegisterMessagePipe();
        }

        protected override void OnDestroy()
        {
            Debug.Log($"[{nameof(MenuScope)}] Destroyed");
            base.OnDestroy();
        }
    }
}
