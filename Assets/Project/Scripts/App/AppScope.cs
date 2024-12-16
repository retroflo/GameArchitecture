using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LWFlo.App
{
    public class AppScope : LifetimeScope, IStartable
    {
        [SerializeField] private AppScopeConfiguration _config;

        public void Start()
        {
            AppStartup();
        }

        private void AppStartup()
        {
            // Entry point - where the adventure begins
            var appManager = Container.Resolve<AppManager>();
            appManager.Start();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            Application.targetFrameRate = 60;
            
            builder.Register<ChildScopeService>(Lifetime.Singleton)
                .WithParameter(_config.childScopeConfiguration)
                .AsImplementedInterfaces();

            builder.Register<AppManager>(Lifetime.Singleton)
                .AsSelf();
        }
    }
}