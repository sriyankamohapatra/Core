using System.Collections;
using System.Reflection;

namespace Sfa.Core.Equality
{
    /// <summary>
    /// Defines an instance which can handle specific comparison within the field value 
    /// equality comparer.
    /// </summary>
    public interface IFieldValueEqualityComparer : IEqualityComparer
    {
        /// <summary>
        /// Returns a flag indicating if this instance can compare the two objects provided.
        /// </summary>
        /// <param name="lhs">The left hand side of the comparison.</param>
        /// <param name="rhs">The right hand side of the comparison.</param>
        /// <returns><c>true</c> if this instance can compare; otherwise, <c>false</c>.</returns>
        bool CanCompare(object lhs, object rhs);

        /// <summary>
        /// Returns a flag indicating if this instance can compare the two objects provided.
        /// </summary>
        /// <param name="lhsField">The left hand side of the comparison.</param>
        /// <param name="rhsField">The right hand side of the comparison.</param>
        /// <param name="lhsParent">The left hand side's parent object of the comparison.</param>
        /// <param name="rhsParent">The right hand side's parent object of the comparison.</param>
        /// <param name="field">The field access used to get to the field object.</param>
        /// <returns><c>true</c> if this instance can compare; otherwise, <c>false</c>.</returns>
        bool CanCompare(ref object lhsField, ref object rhsField, object lhsParent, object rhsParent, FieldInfo field);
    }
}