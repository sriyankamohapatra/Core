using NSubstitute.Core;
using NSubstitute.Routing;

namespace Sfa.Core.Equality
{
    /// <summary>
    /// Overridden version of the default so that we can insert our equality provider.
    /// </summary>
    public class CallRouterFactory : ICallRouterFactory
    {
        /// <summary>
        /// Creates a <see cref="ICallRouter"/> implementation.
        /// </summary>
        /// <param name="substitutionContext">The context.</param>
        /// <param name="config">The selected configuration.</param>
        /// <returns>The created implementation.</returns>
        public ICallRouter Create(ISubstitutionContext substitutionContext, SubstituteConfig config)
        {
            var substituteState = new SubstituteState(substitutionContext, config)
                .UpdateArgumentEqualsSpecificationFactory(new ArgumentFieldValueEqualsSpecificationFactory());
            return new CallRouter(substituteState, substitutionContext, new RouteFactory());
        }
    }
}