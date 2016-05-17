using System;
using System.Collections.Generic;
using Sfa.Core.Logging;
using Sfa.Core.Context;
using System.Linq;
using System.Net.Http;
using Sfa.Core.Threading;

namespace Sfa.Core.Diagnostics
{
    /// <summary>
    /// Runs a health check tests.  
    /// </summary>
    public class HealthCheck
    {
        private readonly IList<bool> _results = new List<bool>();

        /// <summary>
        /// Returns true if all health check tests have passed.
        /// </summary>
        public bool AllPAssed
        {
            get { return _results.All(x => x); }
        }

        /// <summary>
        /// Runs the passed health check test.  If the test does not raise an exception then it logs that the check passed.  If the check raised an exception then the check is logged as failed
        /// and the exception is logged to the log.  
        /// </summary>
        /// <param name="test">The test to run.</param>
        /// <param name="name">The name of the test that gets written to the log.</param>
        public void RunTest(Action test, string name)
        {
            try
            {
                test();
                ApplicationContext.Logger.Log(LoggingLevel.Info, "Health Check", () => $"{name} passed");
                _results.Add(true);
            }
            catch (Exception exception)
            {
                ApplicationContext.Logger.Log(LoggingLevel.Info, "Health Check", () => $"{name} failed");
                ApplicationContext.Logger.LogException("Health Check", exception);
                _results.Add(false);
            }
        }

        /// <summary>
        /// Run a health check test against another health check service.
        /// </summary>
        /// <param name="baseUri">The base uri of the health check service being tested.</param>
        /// <param name="serviceHealthCheckPath">The path appended to the <paramref name="baseUri"/> of the health check service api call.</param>
        /// <param name="name">The name of the health check test that gets written to the log.</param>
        public void RunHealthCheckServiceTest(string baseUri, string serviceHealthCheckPath, string name)
        {
            RunTest(() =>
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    var response = client.GetAsync(new Uri(baseUri + serviceHealthCheckPath)).GetSafeResult();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException(
                            $"Health check failed with status code {response.StatusCode}, {response.ReasonPhrase}.");
                    }
                }
            }, name);
        }
    }
}