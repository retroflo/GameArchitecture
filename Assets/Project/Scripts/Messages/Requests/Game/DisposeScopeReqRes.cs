namespace LWFlo
{
    public struct DisposeScopeRequest
    {
        public string childName;
        public bool errorIfNotFound;
    }

    public struct DisposeScopeResponse
    {
        public bool success;
    }
}