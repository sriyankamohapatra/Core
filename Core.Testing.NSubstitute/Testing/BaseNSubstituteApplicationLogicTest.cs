using NSubstitute;
using NSubstitute.Core;
using Sfa.Core.Context;
using Sfa.Core.Equality;
using Sfa.Core.Logging;

namespace Sfa.Core.Testing
{
    /// <summary>
    /// Base class for logical tests that use the NSubstitute mocking framework.
    /// </summary>
    public abstract class BaseNSubstituteApplicationLogicTest : BaseTest
    {
        /// <summary>
        /// Sets up each tests in a consistent manor.
        /// </summary>
        protected override void SetUpEachTest()
        {
            base.SetUpEachTest();
            SubstitutionContext.Current.SubstituteFactory.OverrideRouterFactory();
        }

        
        /// <summary>
        /// Creates the application context for the tests.
        /// </summary>
        protected override void CreateApplicationContext()
        {
            var logger = new MultiLogger(new ConsoleLogger(), new TraceLogger());
            NetworkContext = NewMock<INetworkContext>();

            ApplicationContext.Setup(new StaticContextStorage(), logger, NetworkContext);
        }


        /// <summary>
        /// Creates a new mock of the specified type.
        /// </summary>
        /// <typeparam name="T">The type to mock.</typeparam>
        /// <returns>The new mock.</returns>
        protected T NewMock<T>()
            where T : class
        {
            return Substitute.For<T>();
        }


        #region Core Mocks

        public INetworkContext NetworkContext { get; set; }

        #endregion
    }
}