using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LWFlo
{
    public class MenuScope : LifetimeScope
    {        
        protected override void Configure(IContainerBuilder builder)
        {

        }

        protected override void OnDestroy()
        {
            Debug.Log($"[{nameof(MenuScope)}] Destroyed");
            base.OnDestroy();
        }
    }
}
