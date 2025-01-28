using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Scripts.Menu
{
    public class MenuScope : LifetimeScope
    {
        [SerializeField] private MenuScopeConfiguration _config;
        protected override void Configure(IContainerBuilder builder)
        {
            
        }
    }
}