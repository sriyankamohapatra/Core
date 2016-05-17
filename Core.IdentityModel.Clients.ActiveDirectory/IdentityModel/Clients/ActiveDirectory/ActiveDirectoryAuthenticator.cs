using System.Diagnostics;
using System.Security.Authentication;
using System.Threading;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Sfa.Core.Context;
using Sfa.Core.Logging;

namespace Sfa.Core.IdentityModel.Clients.ActiveDirectory
{
    /// <summary>
    /// Helper class to help with authentication against Active Directory.
    /// </summary>
    public class ActiveDirectoryAuthenticator
    {
        /// <summary>
        /// Makes a call to the Uri to get an authentication result using the supplied credentials.
        /// </summary>
        /// <param name="uri">The location of the Active Directory Tenant.</param>
        /// <param name="clientid">The client id for the application to be authenticated against.</param>
        /// <param name="clientSecret">The client id for the application to be authenticated against.</param>
        /// <param name="applicationId">The application id of the calling application.</param>
        /// <returns>The authentication result.</returns>
        public static AuthenticationResult GetAuthenticationResult(string uri, string clientid, string clientSecret, string applicationId)
        {
            var authContext = new AuthenticationContext(uri);
            var clientCredential = new ClientCredential(clientid, clientSecret);

            AuthenticationResult authResult = null;
            var retryCount = 0;
            bool retry;

            do
            {
                retry = false;
                try
                {
                    ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => "Getting Authentication Result form {0}, for application {1}", uri, applicationId);

                    var sw = Stopwatch.StartNew();

                    authResult = authContext.AcquireTokenAsync(applicationId, clientCredential).Result;

                    sw.Stop();
                    ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => "Successful call to authentication site {0} (Don't read: got correct token) took {1}ms", uri, sw.ElapsedMilliseconds);

                    return authResult;
                }
                catch (AdalException ex)
                {
                    if (ex.ErrorCode == "temporarily_unavailable")
                    {
                        retry = true;
                        retryCount++;
                        Thread.Sleep(3000);
                    }
                }
            }
            while (retry && (retryCount < 3));

            if (authResult == null)
            {
                if (retryCount > 0)
                {
                    throw new AuthenticationException("Could not authenticate with the OAuth 2.0 claims provider after several attempts.");
                }
                throw new AuthenticationException("Could not authenticate with the OAuth 2.0 claims provider.");
            }

            return authResult;
        }
    }
}