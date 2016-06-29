using System;
using System.Net.Http;

namespace Sfa.Core.Diagnostics
{
    public static class HealthCheckExtentions
    {
        /// <summary>
        /// Run a health check test against another health check service.
        /// </summary>
        /// <param name="healthCheck">The <see cref="HealthCheck"/> instance to use to run the actual test.</param>
        /// <param name="baseUri">The base uri of the health check service being tested.</param>
        /// <param name="serviceHealthCheckPath">The path appended to the <paramref name="baseUri"/> of the health check service api call.</param>
        /// <param name="name">The name of the health check test that gets written to the log.</param>
        public static void RunHealthCheckServiceTest(this HealthCheck healthCheck, string baseUri, string serviceHealthCheckPath, string name)
        {
            healthCheck.RunTest(() =>
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    var response = client.GetAsync(new Uri(baseUri + serviceHealthCheckPath)).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"Health check failed with status code {response.StatusCode}, {response.ReasonPhrase}.");
                    }
                }
            }, name);
        }
    }
}