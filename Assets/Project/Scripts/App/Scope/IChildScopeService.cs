using System;
using LWFlo.Messages;
using MessagePipe;
using VContainer.Unity;

namespace LWFlo.App
{
    public interface IChildScopeService :         
        IRequestHandler<CreateScopeRequest, CreateScopeResponse>,
        IRequestHandler<FetchScopeRequest, FetchScopeResponse>,
        IRequestHandler<DisposeScopeRequest, DisposeScopeResponse>
    {
        LifetimeScope CreateChildScope(LifetimeScope parentScope, string childName, Action<LifetimeScope> setDynamicConfigMethodPreBuild, Action<LifetimeScope> setDynamicConfigMethodPostBuild);
        bool DisposeScope(string childName, bool throwIfNotFound);
    }
}