using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Testing;

namespace Sfa.Core.Text
{
    [TestClass]
    public class StringExtensionsTests : BaseTest
    {
        #region IsValidEmail

        [TestMethod, TestCategory("Unit")]
        public void IsValidEmail()
        {
            var validEmailAddresses = new []{ "david.jones@proseware.com", "d.j@server1.proseware.com",
                                  "jones@ms1.proseware.com",
                                  "j@proseware.com9", "js#internal@proseware.com",
                                  "j_9@[129.126.118.1]", 
                                  "js@proseware.com9", "j.s@server1.proseware.com",
                                   "\"j\\\"s\\\"\"@proseware.com", "js@contoso.中国" };

            var invalidEmailAddresses = new[]{ "j.@server1.proseware.com", "j..s@proseware.com", "js*@proseware.com", "js@proseware..com" };

            foreach (var email in validEmailAddresses)
            {
                Assert.IsTrue(email.IsValidEmail());
            }

            foreach (var email in invalidEmailAddresses)
            {
                Assert.IsFalse(email.IsValidEmail());
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void IsValidEmail_EmptyEmail()
        {
            // Act
            var actual = string.Empty.IsValidEmail();

            // Assert
            actual.ShouldHaveSameValueAs(false);
        }

        #endregion
    }
}
