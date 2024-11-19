using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LWFlo.Project.Scripts.Messages.Requests.Game;
using LWFlo.Project.Scripts.Tools;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace RetroCloud.BioVirus.App
{
    [UsedImplicitly]
    public class ChildScopeService : IChildScopeService
    {
        private readonly Dictionary<string,LifetimeScope> _scopePrefabs;
        private readonly Dictionary<string,LifetimeScope> _activeScopes;

        [Inject]
        public ChildScopeService(ChildScopeConfiguration config)
        {
            _scopePrefabs = config.scopes.ToDictionary(v => v.name, v => v);
            _activeScopes = new Dictionary<string, LifetimeScope>();
        }

        public LifetimeScope CreateChildScope(LifetimeScope parentScope, string childName, 
            Action<LifetimeScope> setDynamicConfigMethodPreBuild, Action<LifetimeScope> setDynamicConfigMethodPostBuild)
        {
            if (parentScope.IsNull())
                throw new InvalidOperationException(
                    $"Trying to create child scope {childName}, but provided parent is null");

            Debug.Log($"[ChildScopeService] Creating child scope {parentScope.name}->{childName} "); // todo: replace debug

            if (_scopePrefabs.ContainsKey(childName) == false)
                throw new KeyNotFoundException($"Scope with name {childName} not found in scope configuration");

            var scopePrefab = _scopePrefabs[childName];
            if (_activeScopes.ContainsKey(childName))
            {
                // Handle special case where a scope can be implicitly destroyed (by destroying it's parent scope directly e.g.)
                // We can infer this by checking of the underlying game object is null and simply cleanup silently and continue
                if (_activeScopes[childName].IsNotNull())
                    throw new InvalidOperationException(
                        $"Trying to create a child scope {childName} while one is already existing!");

                _activeScopes.Remove(childName);
            }

            var instance = parentScope.CreateChildFromPrefab(scopePrefab);
            instance.name = instance.name.Replace("(Clone)", "");
            
            setDynamicConfigMethodPreBuild?.Invoke(instance);
            
            instance.Build();

            _activeScopes.Add(childName, instance);

            setDynamicConfigMethodPostBuild?.Invoke(instance);
            
            return instance;
        }

        public LifetimeScope FetchChildScope(string childName)
        {
            if (_activeScopes.ContainsKey(childName))
                return _activeScopes[childName];

            throw new KeyNotFoundException($"Could not fetch scope {childName} because it was not created!");
        }
        
        public bool DisposeScope(string childName, bool throwIfNotFound = true)
        {
            Debug.Log($"[ChildScopeService] Disposing child scope {childName}"); // todo: replace debug

            if (_activeScopes.ContainsKey(childName) == false)
            {
                if (throwIfNotFound)
                    throw new KeyNotFoundException($"Scope with name {childName} not found in active scopes");
                else
                    return false;
            }
              
            var instance = _activeScopes[childName];
            if (instance.IsNotNull())
                UnityEngine.Object.Destroy(instance.gameObject);

            _activeScopes.Remove(childName);
            return true;
        }

        CreateScopeResponse IRequestHandlerCore<CreateScopeRequest, CreateScopeResponse>.Invoke(CreateScopeRequest request)
        {
            var scope = CreateChildScope( request.parentScope, request.childName, request.setDynamicConfigMethodPreBuild, request.setDynamicConfigMethodPostBuild );
            return new CreateScopeResponse {childScope = scope};
        }
        
        FetchScopeResponse IRequestHandlerCore<FetchScopeRequest, FetchScopeResponse>.Invoke(FetchScopeRequest request)
        {
            var scope = FetchChildScope(request.scopeName);
            return new FetchScopeResponse {scope = scope};
        }

        DisposeScopeResponse IRequestHandlerCore<DisposeScopeRequest, DisposeScopeResponse>.Invoke(DisposeScopeRequest request)
        {
            var result = DisposeScope(request.childName, request.errorIfNotFound);
            return new DisposeScopeResponse {success = result};
        }
    }
    
}