using System;
using System.Reflection;

namespace Sfa.Core.Reflection
{
    /// <summary>
    /// Extensions available for <see cref="MemberInfo"/>.
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Returns a flag indicating is a custom attribute exists on the target.
        /// </summary>
        /// <typeparam name="T">The type of the attribute that is being looked for</typeparam>
        /// <param name="memberInfo">The target of the search.</param>
        /// <param name="inherit">true to search this member's inheritance chain to find the attributes; 
        /// otherwise, false. This parameter is ignored for properties and events; see Remarks.</param>
        /// <returns><c>true</c> if the attribute exists; otherwise, <c>false</c>.</returns>
        public static bool IsDefined<T>(this MemberInfo memberInfo, bool inherit = true) where T : Attribute
        {
            return memberInfo.IsDefined(typeof(T), inherit);
        }
    }
}