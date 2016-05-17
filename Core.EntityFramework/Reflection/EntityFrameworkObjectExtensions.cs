using System.Data.Entity.Core.Objects;

namespace Sfa.Core.Reflection
{
    /// <summary>
    /// Object extensions based around Entity Framework.
    /// </summary>
    public static class EntityFrameworkObjectExtensions
    {
        /// <summary>
        /// Returns a flag determining if the object is an Entity Framework Proxy object.
        /// </summary>
        /// <param name="type">The instance to check against.</param>
        /// <returns><c>true</c> if the instance is a proxy; otherwise, <c>false</c>.</returns>
        public static bool IsProxy(this object type)
        {
            return type != null && ObjectContext.GetObjectType(type.GetType()) != type.GetType();
        }
    }
}