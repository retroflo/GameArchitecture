using LWFlo.States;
using VContainer;
using VContainer.Unity;

namespace LWFlo
{
    public class GameScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameStateGameplayOrResume>(Lifetime.Transient);
            
            builder.Register<GameManager>(Lifetime.Singleton)
                .As<IGameManager>();
        }
    }
}