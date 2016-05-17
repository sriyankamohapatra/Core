using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sfa.Core.Json
{
    /// <summary>
    /// Extends <see cref="DefaultContractResolver"/> with the ability to write to non public setters for properties
    /// </summary>
    public class NonPublicPropertiesResolver : DefaultContractResolver
    {
        /// <summary>
        /// Implementation for writing to a property.
        /// </summary>
        /// <param name="member">The member to write to.</param>
        /// <param name="memberSerialization">The serialised version of the property.</param>
        /// <returns>The created json property.</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            var pi = member as PropertyInfo;
            if (pi != null)
            {
                prop.Readable = (pi.GetMethod != null);
                prop.Writable = (pi.SetMethod != null);
            }
            return prop;
        }
    }
}