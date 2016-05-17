using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sfa.Core.Reflection
{
    /// <summary>
    /// Extensions for <see cref="ICustomAttributeProvider"/>.
    /// </summary>
    public static class CustomAttributeProviderExtensions
    {
        /// <summary>
        /// Returns a flag stating whther or not the member has the specified attribute applied to it.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to find.</typeparam>
        /// <param name="customAttributeProvider">The member to look on for the attribute.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns><c>true</c> if the attribute is found.</returns>
        public static bool HasAttribute<T>(this ICustomAttributeProvider customAttributeProvider, bool inherit = true)
        {
            return customAttributeProvider.GetCustomAttributes(typeof(T), inherit).Length > 0;
        }

        /// <summary>
        /// Returns all the attribute of the specified type found on the member.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to find.</typeparam>
        /// <param name="customAttributeProvider">The member to look on for the attribute.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>All matching attributes.</returns>
        public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider customAttributeProvider, bool inherit = true)
        {
            return customAttributeProvider.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }

        /// <summary>
        /// Gets a single attribute defined on the member of the specified type. 
        /// </summary>
        /// <typeparam name="T">The type of the attribute to find.</typeparam>
        /// <param name="customAttributeProvider">The member to look on for the attribute.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <param name="index">If multiple attributes are permitted, then an optional index can be supplied if the first attribute isn't the required one.</param>
        /// <returns>The attribute.</returns>
        public static T GetAttribute<T>(this ICustomAttributeProvider customAttributeProvider, bool inherit = true, int index = 0)
        {
            return customAttributeProvider.GetAttributes<T>(inherit).ToList()[index];
        }
    }
}