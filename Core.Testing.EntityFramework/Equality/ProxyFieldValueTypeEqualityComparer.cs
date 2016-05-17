using Sfa.Core.Reflection;

namespace Sfa.Core.Equality
{
    public class ProxyFieldValueTypeEqualityComparer :IFieldValueTypeEqualityComparer
    {
        /// <summary>
        /// Returns a flag if the two objects should be treated as though they were the same type even though they might not be.
        /// </summary>
        /// <param name="lhs">The left hand side of the comparison.</param>
        /// <param name="rhs">The right hand side of the comparison.</param>
        /// <returns><c>true</c> if the two objects should be treated as the same type; otherwise, <c>false</c>.</returns>
        public bool TreatAsSameType(object lhs, object rhs)
        {
            if (lhs != null && rhs != null)
            {
                var lhsType = lhs.GetType();
                if (lhs.IsProxy())
                {
                    lhsType = lhs.GetType().BaseType;
                }
                var rhsType = rhs.GetType();
                if (rhs.IsProxy())
                {
                    rhsType = rhs.GetType().BaseType;
                }
                return lhsType == rhsType;
            }
            return false;
        }
    }
}