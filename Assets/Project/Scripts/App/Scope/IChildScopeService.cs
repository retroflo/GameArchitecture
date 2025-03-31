using System;
using MessagePipe;
using VContainer.Unity;

namespace LWFlo
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