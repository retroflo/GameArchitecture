using VContainer;
using VContainer.Unity;

namespace LWFlo.Game
{
    public class GameScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<LocalDataStorage>(Lifetime.Singleton)
                .As<ILocalDataStorage>();
            
            builder.Register<GameStateGameplayOrMenu>(Lifetime.Transient);
            
            builder.Register<GameManager>(Lifetime.Singleton)
                .As<IGameManager>();
        }
    }
}