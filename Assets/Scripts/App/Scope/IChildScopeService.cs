using System;
using LWFlo.Project.Scripts.Messages.Requests.Game;
using MessagePipe;
using VContainer.Unity;

namespace RetroCloud.BioVirus.App
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