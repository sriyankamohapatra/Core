using System;
using System.Collections.Generic;
using Sfa.Core.Logging;
using Sfa.Core.Context;
using System.Linq;

namespace Sfa.Core.Diagnostics
{
    /// <summary>
    /// Runs a health check tests.  
    /// </summary>
    public class HealthCheck
    {
        private readonly IList<bool> _results = new List<bool>();

        /// <summary>
        /// Returns <c>true</c> if all health check tests have passed.
        /// </summary>
        public bool AllPassed
        {
            get { return _results.All(x => x); }
        }

        /// <summary>
        /// Runs the passed health check test.  If the test does not raise an exception then it logs that the check passed. If the check raised an exception then the check is logged as failed
        /// and the exception is logged to the log.  
        /// </summary>
        /// <param name="test">The test to run.</param>
        /// <param name="name">The name of the test that gets written to the log.</param>
        public void RunTest(Action test, string name)
        {
            try
            {
                test();
                ApplicationContext.Logger.Log(LoggingLevel.Info, CoreLoggingCategory.HealthCheck, () => $"{name} passed");
                _results.Add(true);
            }
            catch (Exception exception)
            {
                ApplicationContext.Logger.Log(LoggingLevel.Info, CoreLoggingCategory.HealthCheck, () => $"{name} failed");
                ApplicationContext.Logger.LogException(CoreLoggingCategory.HealthCheck, exception);
                _results.Add(false);
            }
        }
    }
}