using System;
using System.Collections.Generic;

namespace Sfa.Core.Equality
{
    /// <summary>
    /// Class intended for use in dictionaries etc. where reference comparison of keys is important.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> to compare.</typeparam>
    public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="T:System.NullReferenceException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is <c>null</c>.</exception>
        public int GetHashCode(T obj)
        {
            if (ReferenceEquals(obj, null))
            {
                throw new NullReferenceException();
            }

            return obj.GetHashCode();
        }
    }
}