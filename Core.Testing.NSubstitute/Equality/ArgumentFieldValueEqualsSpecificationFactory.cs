using System;
using NSubstitute.Core.Arguments;

namespace Sfa.Core.Equality
{
    /// <summary>
    /// Factory class for creating <see cref="IArgumentSpecification"/> implementations.
    /// </summary>
    public class ArgumentFieldValueEqualsSpecificationFactory : IArgumentEqualsSpecificationFactory
    {
        /// <summary>
        /// Create a new implementation of <see cref="IArgumentSpecification"/>.
        /// </summary>
        /// <param name="value">The value to compare against.</param>
        /// <param name="forType">The type of the object to compare on.</param>
        /// <returns>The new instance.</returns>
        public IArgumentSpecification Create(object value, Type forType)
        {
            return new ArgumentSpecification(forType, new FieldValueEqualsArgumentMatcher(value));
        }
    }
}