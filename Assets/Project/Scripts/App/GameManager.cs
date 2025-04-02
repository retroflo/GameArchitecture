using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace LWFlo
{
    /// <summary>
    /// GameManager handles switching between game states (menu and gameplay)
    /// and manages the game's overall state.
    /// </summary>
    public class GameManager
    {
        private readonly LifetimeScope _currentScope;
        private readonly IRequestHandler<CreateScopeRequest, CreateScopeResponse> _scopeCreator;
        private readonly IChildScopeService _childScopeService;
        private readonly LifetimeScope _parentScope;
        
        private ScopeName _currentScopeName;

        [Inject]
        public GameManager(IChildScopeService childScopeService, LifetimeScope parentScope, 
            IRequestHandler<CreateScopeRequest, CreateScopeResponse> scopeCreator, LifetimeScope currentScope)
        {
            _childScopeService = childScopeService;
            _parentScope = parentScope;
            _scopeCreator = scopeCreator;
            _currentScope = currentScope;
        }

        public void Initialize()
        {
            _scopeCreator.Invoke(new CreateScopeRequest
                { childName = ScopeName.MenuScope.ToString(), parentScope = _currentScope });
            
            Debug.Log($"{nameof(GameManager)} {ScopeName.MenuScope} created");
        }

        /// <summary>
        /// Switches to the menu scope.
        /// </summary>
        public void SwitchToMenuScope()
        {
            SwitchScope(ScopeName.MenuScope);
        }

        /// <summary>
        /// Switches to the gameplay scope.
        /// </summary>
        public void SwitchToGameplayScope()
        {
            SwitchScope(ScopeName.GameplayScope);
        }
        
        private void SwitchScope(ScopeName scopeName)
        {
            var currentScopeToString = _currentScopeName.ToString();
            var newScopeToString = scopeName.ToString();
            // Dispose current scope if it exists
            if (string.IsNullOrEmpty(currentScopeToString) == false)
                _childScopeService.DisposeScope(currentScopeToString, false);

            // Create new scope
            _childScopeService.CreateChildScope(_parentScope, newScopeToString, null, null);
            _currentScopeName = scopeName;
            
            Debug.Log($"[{nameof(GameManager)}] Switched to scope: {newScopeToString}");
        }
    }
}
