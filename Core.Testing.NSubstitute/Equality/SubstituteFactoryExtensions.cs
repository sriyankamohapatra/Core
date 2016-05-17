using NSubstitute.Core;
using Sfa.Core.Reflection;

namespace Sfa.Core.Equality
{
    /// <summary>
    /// Adds more abilities to <see cref="ISubstituteFactory"/>.
    /// </summary>
    public static class SubstituteFactoryExtensions
    {
        /// <summary>
        /// Overrides the default implementation of CallRouterFactory with ours so we can enable field value testing.
        /// </summary>
        /// <param name="substituteFactory">the instance to override on.</param>
        /// <returns>The supplied value for method chaining.</returns>
        public static ISubstituteFactory OverrideRouterFactory(this ISubstituteFactory substituteFactory)
        {
            return substituteFactory.SetPrivateFieldValue("_callRouterFactory", new CallRouterFactory());
        }
    }
}