using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using Sfa.Core.Reflection;

namespace Sfa.Core.Equality
{
    /// <summary>
    /// Adds capabilities to <see cref="SubstituteState"/>.
    /// </summary>
    public static class SubstituteStateExtensions
    {
        /// <summary>
        /// Updates the <see cref="IArgumentEqualsSpecificationFactory"/> from the defaults.
        /// </summary>
        /// <param name="substituteState">The instance to update.</param>
        /// <param name="argumentEqualsSpecificationFactory">The new factory to use.</param>
        /// <returns>The updates state so that method chaining can occur.</returns>
        public static SubstituteState UpdateArgumentEqualsSpecificationFactory(this SubstituteState substituteState, IArgumentEqualsSpecificationFactory argumentEqualsSpecificationFactory)
        {
            var argumentSpecificationFactory = substituteState
                .CallSpecificationFactory
                .GetPrivateFieldValue("_argumentSpecificationsFactory")
                .GetPrivateFieldValue("_mixedArgumentSpecificationsFactory")
                .GetPrivateFieldValue("_argumentSpecificationFactory");

            argumentSpecificationFactory
                .GetPrivateFieldValue("_paramsArgumentSpecificationFactory")
                .SetPrivateFieldValue("_argumentEqualsSpecificationFactory", argumentEqualsSpecificationFactory)
                .GetPrivateFieldValue("_arrayArgumentSpecificationsFactory")
                .GetPrivateFieldValue("_nonParamsArgumentSpecificationFactory")
                .SetPrivateFieldValue("_argumentEqualsSpecificationFactory", argumentEqualsSpecificationFactory);

            argumentSpecificationFactory
                .GetPrivateFieldValue("_nonParamsArgumentSpecificationFactory")
                .SetPrivateFieldValue("_argumentEqualsSpecificationFactory", argumentEqualsSpecificationFactory);

            return substituteState;
        }
    }
}