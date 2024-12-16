using VContainer.Unity;

namespace LWFlo.Messages
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