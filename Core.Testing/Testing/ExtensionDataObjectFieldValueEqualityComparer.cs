using System.Reflection;
using System.Runtime.Serialization;
using Sfa.Core.Equality;

namespace Sfa.Core.Testing
{
    /// <summary>
    /// When comparing objects returned by a wcf call, they may include the <see cref="ExtensionDataObject"/> object. If you don't care about this, then
    /// use this type to ignore them.
    /// </summary>
    public class ExtensionDataObjectFieldValueEqualityComparer : IFieldValueEqualityComparer
    {
        /// <summary>
        /// Override of the default equality for mocked instances.
        /// </summary>
        /// <param name="lhs">The left hand side of the comparison.</param>
        /// <param name="rhs">The right hand side of the comparison.</param>
        /// <returns><c>true</c></returns>
        public new bool Equals(object lhs, object rhs)
        {
            return true;
        }

        public int GetHashCode(object obj)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns a flag indicating if this instance can compare the two objects provided.
        /// </summary>
        /// <param name="lhs">The left hand side of the comparison.</param>
        /// <param name="rhs">The right hand side of the comparison.</param>
        /// <returns><c>true</c> if this instance can compare; otherwise, <c>false</c>.</returns>
        public bool CanCompare(object lhs, object rhs)
        {
            return (lhs is ExtensionDataObject) || (rhs is ExtensionDataObject);
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
            return (lhsField is ExtensionDataObject) || (rhsField is ExtensionDataObject);
        }
    }
}