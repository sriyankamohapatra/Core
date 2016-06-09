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
        /// Returns a flag indicating if a custom attribute exists on the target <see cref="MemberInfo"/>.
        /// </summary>
        /// <typeparam name="T">The type of the attribute that is being looked for</typeparam>
        /// <param name="memberInfo">The target of the search.</param>
        /// <param name="inherit">true to search this member's inheritance chain to find the attributes; 
        /// otherwise, false. This parameter is ignored for properties and events; see Remarks.</param>
        /// <returns><c>true</c> if the attribute exists; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="memberInfo"/> is <c>null</c>.</exception>
        public static bool IsDefined<T>(this MemberInfo memberInfo, bool inherit = true) where T : Attribute
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            return memberInfo.IsDefined(typeof(T), inherit);
        }
    }
}