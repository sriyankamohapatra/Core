using System.Net.Http;
using System.Net.Http.Headers;
using Sfa.Core.IdentityModel.Clients.ActiveDirectory;

namespace Sfa.Core.Net.Http
{
    /// <summary>
    /// Adds extension methods to the <see cref="HttpClient"/> class.
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Adds the Bearer authorisation token from Active Directory for the specified credentials.
        /// </summary>
        /// <param name="client">The client to add the authorisation token to.</param>
        /// <param name="uri">The location of the Active Directory Tenant.</param>
        /// <param name="clientid">The client id for the application to be authenticated against.</param>
        /// <param name="clientSecret">The client id for the application to be authenticated against.</param>
        /// <param name="applicationId">The application id of the calling application.</param>
        /// <returns>The client instance.</returns>
        public static HttpClient AuthenticateAgainstActiveDirectory(this HttpClient client, string uri, string clientid, string clientSecret, string applicationId)
        {
            var result = ActiveDirectoryAuthenticator.GetAuthenticationResult(uri, clientid, clientSecret, applicationId);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            return client;
        }
    }
}