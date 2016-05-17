using System.Collections;

namespace Sfa.Core
{
    /// <summary>
    ///  Extension available of the byte Array class.
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Compares two objects for structural equality
        /// </summary>
        /// <param name="lhs">The instance to compare against.</param>
        /// <param name="rhs">The instance being compared.</param>
        /// <returns><c>True</c> if specified instances are equal, otherwise <c>False</c>.</returns>
        public static bool StructurallyEqual(this byte[] lhs, byte[] rhs)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(lhs, rhs);
        }
    }
}
