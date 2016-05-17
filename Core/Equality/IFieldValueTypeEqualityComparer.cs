namespace Sfa.Core.Equality
{
    /// <summary>
    /// Describes the interface where two objects should be treated as though they are the same type.
    /// </summary>
    public interface IFieldValueTypeEqualityComparer
    {
        /// <summary>
        /// Returns a flag if the two objects should be treated as though they were the same type even though they might not be.
        /// </summary>
        /// <param name="lhs">The left hand side of the comparison.</param>
        /// <param name="rhs">The right hand side of the comparison.</param>
        /// <returns><c>true</c> if the two objects should be treated as the same type; otherwise, <c>false</c>.</returns>
        bool TreatAsSameType(object lhs, object rhs);
    }
}