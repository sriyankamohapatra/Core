using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Context;
using Sfa.Core.Equality;
using Sfa.Core.Logging;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace Sfa.Core.Testing
{
    /// <summary>
    /// Base test class for all tests.
    /// Uses MSTest
    /// </summary>
    public abstract class BaseTest
    {
        #region Properties
        
        public TestContext TestContext { get; set; }

        #endregion



        #region Plumbing

        /// <summary>
        /// Performs set up for each test.
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            SetUpEachTest();
        }

        /// <summary>
        /// Performs clean up for each test.
        /// </summary>
        [TestCleanup]
        public void TearDown()
        {
            TearDownEachTest();
        }

        /// <summary>
        /// Creates the application context for the tests.
        /// </summary>
        protected virtual void CreateApplicationContext()
        {
            var logger = new MultiLogger(LogggersToUse.ToArray());

            ApplicationContext.Setup(new StaticContextStorage(), 
                                     logger, 
                                     new AdjustableNetworkContext(o => o, new NumericallyIncrementingGuidProvider().SetSeedValue(1000)));
        }

        protected virtual IEnumerable<ILogger> LogggersToUse
        {
            get
            {
                yield return new TraceLogger();
            }
        }

        /// <summary>
        /// Tears down the application context for the tests.
        /// </summary>
        protected static void TearDownApplicationContext()
        {
            ApplicationContext.TearDown();
        }

        /// <summary>
        /// Override this to alter the standard flow of set up for each test.
        /// </summary>
        protected virtual void SetUpEachTest()
        {
            FieldValueEqualityComparer.UseDateTimeEqualityComparer();
            SetAssembliesWithTypesToPerformFieldValueEqualityOn();
            CreateApplicationContext();
            EnableLogging(typeof(BaseTest).FullName);
            EnableComparisonLogging();
        }

        /// <summary>
        /// Override this to alter the standard flow of clean up for each test.
        /// </summary>
        protected virtual void TearDownEachTest()
        {
            TearDownApplicationContext();
            ClearAssembliesWithTypesToPerformFieldValueEqualityOn();
        }

        #endregion Plumbing


        #region Comparison

        /// <summary>
        /// Sets up comparison logging for the assemblies returned from <see cref="AssembliesWithTypesToPerformFieldValueEqualityOn"/>.
        /// </summary>
        protected void SetAssembliesWithTypesToPerformFieldValueEqualityOn()
        {
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(AssembliesWithTypesToPerformFieldValueEqualityOn.ToList());
        }

        /// <summary>
        /// Clears the assemblies for field level equality testing.
        /// </summary>
        protected void ClearAssembliesWithTypesToPerformFieldValueEqualityOn()
        {
            FieldValueEqualityComparer.SetAssembliesWithTypesToUseValueSemanticsOn(new Assembly[0]);
        }

        /// <summary>
        /// Override this to supply this list of assemblies whose types will have field level equality performed on them.
        /// </summary>
        protected virtual IEnumerable<Assembly> AssembliesWithTypesToPerformFieldValueEqualityOn => new Assembly[0];

        #endregion Comparison


        #region Logging Helpers

        /// <summary>
        /// Simple logging during a test.
        /// </summary>
        /// <param name="messageFormat">The message to log.</param>
        /// <param name="args">Arguments for the message.</param>
        public void Log(string messageFormat, params object[] args)
        {
            ApplicationContext.Logger.Log(LoggingLevel.Debug, typeof(BaseTest).FullName, () => messageFormat, args);
        }

        /// <summary>
        /// Turns on comparison logging.
        /// </summary>
        public void EnableComparisonLogging()
        {
            EnableLogging(typeof(FieldValueEqualityComparer).FullName);
        }

        /// <summary>
        /// Turns off comparison logging.
        /// </summary>
        public void DisbleComparisonLogging()
        {
            DisbleLogging(typeof(FieldValueEqualityComparer).FullName);
        }

        /// <summary>
        /// Turns on logging for the specified category.
        /// </summary>
        /// <param name="category">The category to log against.</param>
        public void EnableLogging(string category)
        {
            ApplicationContext.Logger.SetCategoryLogging(category, LoggingLevel.Debug);
        }

        /// <summary>
        /// Turns off logging for the specified category.
        /// </summary>
        /// <param name="category">The category to turn logging off for.</param>
        public void DisbleLogging(string category)
        {
            ApplicationContext.Logger.SetCategoryLogging(category, LoggingLevel.None);
        }

        /// <summary>
        /// Turns on logging for the builders.
        /// </summary>
        public void EnableBuilderLogging()
        {
            EnableLogging("Builder");
        }

        #endregion Logging Helpers


        #region Exception Handling

        /// <summary>
        /// Helper methods to assert against exceptions.
        /// </summary>
        /// <typeparam name="T">The type of exception expected from the action.</typeparam>
        /// <param name="action">The action that should thrown an exception.</param>
        /// <returns>The thrown exception.</returns>
        protected T ExpectThrows<T>(Action action)
            where T : Exception
        {
            try
            {
                action();
                Assert.Fail("Expected exception of type {0}", typeof(T));
            }
            catch (Exception exception)
            {
                var expectedException = exception as T;
                if (expectedException != null)
                {
                    return expectedException;
                }
                Assert.Fail("Expected exception of type {0} but got {1}", typeof(T), exception);
            }
            return null;
        }

        #endregion Exception Handling
    }
}