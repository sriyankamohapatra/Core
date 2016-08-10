using System;
using System.Collections.Generic;

namespace Sfa.Core.Equality
{
    /// <summary>
    /// Implements default comparison of <see cref="DateTime"/>s.
    /// </summary>
    public class DefaultDateTimeEqualityComparer : IEqualityComparer<DateTime>
    {
        /// <summary>Determines whether the specified <see cref="DateTime"/>s are equal.</summary>
        /// <returns>true if the specified <see cref="DateTime"/>s are equal; otherwise, false.</returns>
        /// <param name="x">The first <see cref="DateTime"/> to compare.</param>
        /// <param name="y">The second <see cref="DateTime"/> to compare.</param>
        public bool Equals(DateTime x, DateTime y)
        {
            return x.Equals(y);
        }

        /// <summary>Returns a hash code for the specified <see cref="DateTime"/>.</summary>
        /// <returns>A hash code for the specified <see cref="DateTime"/>.</returns>
        /// <param name="obj">The <see cref="DateTime" /> for which a hash code is to be returned.</param>
        public int GetHashCode(DateTime obj)
        {
            return obj.GetHashCode();
        }
    }
}