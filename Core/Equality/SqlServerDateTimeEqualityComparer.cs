using System;
using System.Collections.Generic;

namespace Sfa.Core.Equality
{
    /// <summary>
    /// Implements default comparison of <see cref="DateTime"/>s for dates that have been stored within a SQL server.
    /// https://msdn.microsoft.com/en-GB/library/ms187819.aspx defines the rounding that can occur.
    /// </summary>
    public class SqlServerDateTimeEqualityComparer : IEqualityComparer<DateTime>
    {
        private readonly IEqualityComparer<DateTime> _innerComparer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="innerComparer">the inner comparer to use. If not supplied, then an instance 
        /// of <see cref="DefaultDateTimeEqualityComparer"/> is used once adjustments of the times have been made.</param>
        public SqlServerDateTimeEqualityComparer(IEqualityComparer<DateTime> innerComparer = null)
        {
            _innerComparer = innerComparer ?? new DefaultDateTimeEqualityComparer();
        }


        /// <summary>Determines whether the specified <see cref="DateTime"/>s are equal.</summary>
        /// <returns>true if the specified <see cref="DateTime"/>s are equal; otherwise, false.</returns>
        /// <param name="x">The first <see cref="DateTime"/> to compare.</param>
        /// <param name="y">The second <see cref="DateTime"/> to compare.</param>
        public bool Equals(DateTime x, DateTime y)
        {
            // SQL server rounds the precision of the milliseconds
            // https://msdn.microsoft.com/en-GB/library/ms187819.aspx
            Func<DateTime, int> secondAdjsuter = dateTime => dateTime.Millisecond == 999 ? 1 : 0;
            Func<DateTime, DateTime> adjuster = dateTime => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second).AddSeconds(secondAdjsuter(dateTime));

            var adjustedDateTimeX = adjuster(x);
            var adjustedDateTimeY = adjuster(y);

            return _innerComparer.Equals(adjustedDateTimeX, adjustedDateTimeY);
        }

        /// <summary>
        /// Returns the standard hash code for a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="obj">The DateTime to get the hash code for.</param>
        /// <returns>The hash code of the instance supplied.</returns>
        public int GetHashCode(DateTime obj)
        {
            return obj.GetHashCode();
        }
    }
}