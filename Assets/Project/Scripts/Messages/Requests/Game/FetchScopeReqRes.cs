using VContainer.Unity;

namespace LWFlo
{
    public struct FetchScopeRequest
    {
        public string scopeName;
    }

    public struct FetchScopeResponse
    {
        public LifetimeScope scope;
    }
}