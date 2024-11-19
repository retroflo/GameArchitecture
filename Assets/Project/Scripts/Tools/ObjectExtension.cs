namespace LWFlo.Project.Scripts.Tools
{
    public static class ObjectExtension
    {
        // MonoBehaviours referenced through interfaces will throw MissingReferenceException if trying to directly compare to null (==)
        // After the object was destroyed. Use Equals instead
        public static bool IsNull(this object obj)
        {
            return obj == null || obj.Equals(null);
        }

        public static bool IsNotNull(this object obj)
        {
            return !IsNull(obj);
        }
    }
}