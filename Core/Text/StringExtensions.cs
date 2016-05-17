using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Sfa.Core.Text
{
    /// <summary>
    /// Adds extensions to the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a flag indicating if the input string is a valid email address.
        /// </summary>
        /// <param name="email">The string to test if an email address.</param>
        /// <returns><c>true</c> if a valid email address; otherwise, <c>false</c>.</returns>
        /// <remarks>Taken from https://msdn.microsoft.com/en-us/library/01escwtf(v=vs.110).aspx </remarks>
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch(ArgumentException)
            {
                return false;
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            // Return true if email is in valid e-mail format.
            try
            {
                return Regex.IsMatch(email,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private static string DomainMapper(Match match)
        {
            var idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            domainName = idn.GetAscii(domainName);
            return match.Groups[1].Value + domainName;
        }
    }
}
