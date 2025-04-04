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
            var appManager = Container.Resolve<AppManager>();
            appManager.Initialize();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            Application.targetFrameRate = 60;
            
            builder.Register<AppManager>(Lifetime.Singleton);
            builder.Register<GameManager>(Lifetime.Singleton);
            builder.Register<AppSceneManager>(Lifetime.Singleton)
                .WithParameter(_config.uiRoot);
            
            // Register GameStates
            builder.Register<MenuGameState>(Lifetime.Singleton)
                .WithParameter(_config.uiMenu);
            builder.Register<GameplayState>(Lifetime.Singleton)
                .WithParameter(_config.uiGameplay);
            
            builder.Register<ChildScopeService>(Lifetime.Singleton)
                .WithParameter(_config.childScopeConfiguration)
                .AsImplementedInterfaces();
        }
    }
}