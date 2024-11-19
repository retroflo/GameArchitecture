using System;
using VContainer.Unity;

namespace LWFlo.Project.Scripts.Messages.Requests.Game
{
    public struct CreateScopeRequest
    {
        public LifetimeScope parentScope;
        public string childName;
        
        public Action<LifetimeScope> setDynamicConfigMethodPreBuild; // Subscribe to dynamic config setup here (before building scope)
        public Action<LifetimeScope> setDynamicConfigMethodPostBuild; // Subscribe to dynamic config setup here (after building scope)
    }

    public struct CreateScopeResponse
    {
        public LifetimeScope childScope;
    }
}