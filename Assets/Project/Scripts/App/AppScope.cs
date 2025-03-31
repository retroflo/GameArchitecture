using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LWFlo
{
    public class AppScope : LifetimeScope
    {
        [SerializeField] private AppScopeConfiguration _config;

        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            base.Awake();
        }

        private void Start()
        {
            AppStartup();
        }

        private void AppStartup()
        {
        }

        protected override void Configure(IContainerBuilder builder)
        {
            Application.targetFrameRate = 60;
            
            builder.Register<ChildScopeService>(Lifetime.Singleton)
                .WithParameter(_config.childScopeConfiguration)
                .AsImplementedInterfaces();
        }
    }
}