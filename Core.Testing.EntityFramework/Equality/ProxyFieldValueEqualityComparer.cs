using System;
using System.Reflection;
using Sfa.Core.Reflection;

namespace Sfa.Core.Equality
{
    public class ProxyFieldValueEqualityComparer : IFieldValueEqualityComparer
    {
        /// <summary>
        /// Returns a flag indicating if this instance can compare the two objects provided.
        /// </summary>
        /// <param name="lhs">The left hand side of the comparison.</param>
        /// <param name="rhs">The right hand side of the comparison.</param>
        /// <returns><c>true</c> if this instance can compare; otherwise, <c>false</c>.</returns>
        public bool CanCompare(object lhs, object rhs)
        {
            return false;
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
            object parent = null;
            var isLhs = true;

            // Force lazy loading of the objects
            if (lhsParent.IsProxy() && lhsField == null && rhsField != null)
            {
                // we want to attempt to force the load of the proxy if applicable
                parent = lhsParent;
            }
            else if (rhsParent.IsProxy() && rhsField == null && lhsField != null)
            {
                // we want to attempt to force the load of the proxy if applicable
                parent = rhsParent;
                isLhs = false;
            }

            if (parent != null)
            {
                // we want to attempt to force the load of the proxy if applicable
                var name = field.Name;
                if (name.StartsWith("<") && name.EndsWith(">k__BackingField"))
                {
                    name = name.Substring(1);
                    name = name.Substring(0, name.IndexOf(">", StringComparison.Ordinal));
                    var prop = parent.GetType().GetProperty(name);

                    // Do nothing with this it will just get lazy loaded.
                    var proxy = prop.GetValue(parent);
                    if (proxy != null)
                    {
                        if (isLhs)
                        {
                            lhsField = proxy;
                        }
                        else
                        {
                            rhsField = proxy;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Override of the default equality for mocked instances.
        /// </summary>
        /// <param name="lhs">The left hand side of the comparison.</param>
        /// <param name="rhs">The right hand side of the comparison.</param>
        /// <returns><c>true</c> if the instances are both mocks; otherwise, <c>false</c>.</returns>
        public new bool Equals(object lhs, object rhs)
        {
            return false;
        }

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }
    }
}