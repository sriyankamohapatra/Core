using System;
using Sfa.Core.Diagnostics;
using System.Web.Http;
using System.Reflection;
using System.Diagnostics;

namespace Sfa.Core.Web.Http.Controllers
{
    /// <summary>
    /// Base controlller to run health check tests.
    /// </summary>
    public class BaseHealthCheckController : ApiController
    {
        /// <summary>
        /// Runs the passed health check tests.  
        /// </summary>
        /// <param name="runHealthCheckTests">Action taking a parameter of type <see cref="HealthCheck"/>.  Each health check included within the action is expected to call the RunTest method of
        /// the passed HealthCheck parameter.</param>
        /// <returns>Returns status OK (200) if all checks have passed, internal server error (500) otherwise.</returns>
        protected IHttpActionResult RunTests(Action<HealthCheck> runHealthCheckTests )
        {
            var healthCheck = new HealthCheck();
            
            runHealthCheckTests(healthCheck);

            if (healthCheck.AllPAssed)
            {

                var assembly = Assembly.GetExecutingAssembly();
                var fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
                return Ok(fileVersion.FileVersion);
            }

            return InternalServerError();
        }
    }
}


