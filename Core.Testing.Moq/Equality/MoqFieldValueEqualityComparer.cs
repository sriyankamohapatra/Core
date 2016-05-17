using System;
using System.Reflection;
using Moq;

namespace Sfa.Core.Equality
{
    /// <summary>
    /// Field value comparer extension for the Moq framework.
    /// </summary>
    public class MoqFieldValueEqualityComparer : IFieldValueEqualityComparer
    {
        /// <summary>
        /// Override of the default equality for mocked instances.
        /// </summary>
        /// <param name="lhs">The left hand side of the comparison.</param>
        /// <param name="rhs">The right hand side of the comparison.</param>
        /// <returns><c>true</c> if the instances are both mocks; otherwise, <c>false</c>.</returns>
        public new bool Equals(object lhs, object rhs)
        {
            if ((lhs is IMocked) && (rhs is IMocked))
            {
                return ReferenceEquals(lhs, rhs);
            }
            else
            {
                // TODO
                return true;
            }
        }

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a flag indicating if this instance can compare the two objects provided.
        /// </summary>
        /// <param name="lhs">The left hand side of the comparison.</param>
        /// <param name="rhs">The right hand side of the comparison.</param>
        /// <returns><c>true</c> if this instance can compare; otherwise, <c>false</c>.</returns>
        public bool CanCompare(object lhs, object rhs)
        {
            return (lhs is IMocked) || (rhs is IMocked);
        }

        /// <summary>
        /// Returns a flag indicating if this instance can compare the two objects provided.
        /// </summary>
        /// <param name="lhsField">The left hand side of the comparison.</param>
        /// <param name="rhsField">The right hand side of the comparison.</param>
        /// <param name="lhsParent">The left hand side's parent object of the comparison.</param>
        /// <param name="rhsParent">The right hand side's parent object of the comparison.</param>
        /// <param name="field">The field access used to get to the field object.</param>
        /// <returns><c>true</c> if this instance can compare; otherwise, <c>false</c>.</returns>
        public bool CanCompare(ref object lhsField, ref object rhsField, object lhsParent, object rhsParent, FieldInfo field)
        {
            return (lhsField is IMocked) || (rhsField is IMocked);
        }
    }
}