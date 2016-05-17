using System;
using System.Linq;
using System.Security.Claims;

namespace Sfa.Core.Security.Claims
{
    /// <summary>
    /// Helper methods for claims principal.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Add a claim to the first identity of the claims principal.
        /// </summary>
        /// <param name="claimsPrincipal">The <c>ClaimsPrincipal</c> the claim is being added to.</param>
        /// <param name="type">The type of the new claim.</param>
        /// <param name="value">The value of the new claim.</param>
        public static void AddClaim(this ClaimsPrincipal claimsPrincipal, string type, string value)
        {
            claimsPrincipal.Identities.First().AddClaim(new Claim(type, value));

            // Above code isn't right as I'm wanting to add to the principal identity but the below code doesn't throw
            // an error but doesn't add the claim either.  The articles I looked at on SO about adding claims do this
            // in conjunction with adding a new identity at the same time.
            //(claimsPrincipal.Identity as ClaimsIdentity).AddClaim(new Claim(type, value));
        }

        /// <summary>
        /// Clear all claims of the passed <paramref name="type"/> from <paramref name="claimsPrincipal"/>.
        /// </summary>
        /// <param name="claimsPrincipal">The <c>ClaimsPrincipal</c> the claims are being cleared from.</param>
        /// <param name="type">The type of claims being cleared.</param>
        public static void ClearAll(this ClaimsPrincipal claimsPrincipal, string type)
        {
            foreach (var claimsIdentity in claimsPrincipal.Identities)
            {
                foreach (var claim in claimsIdentity.Claims.Where(c => c.Type == type))
                {
                    claimsIdentity.RemoveClaim(claim);
                }
            }
        }
    }
}