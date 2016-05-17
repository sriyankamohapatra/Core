using System.Collections.Generic;
using System.Linq;
using LumiSoft.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sfa.Core.Net.Mail
{
    /// <summary>
    /// Extension methods for <see cref="Mail_Message"/>.
    /// </summary>
    public static class Mail_MessageExtensions
    {
        /// <summary>
        /// Checks that the specified SmtpMessage has the expected recipients
        /// </summary>
        /// <param name="smtpMessage">The SMTP message.</param>
        /// <param name="expectedRecipientEmailAddresses">The expected recipient email addresses.</param>
        /// <returns></returns>
        public static Mail_Message ShouldBeTo(this Mail_Message smtpMessage, IList<string> expectedRecipientEmailAddresses)
        {
            var addresses = smtpMessage.To.OfType<Mail_t_Mailbox>().Select(o => o.Address).ToList();

            Assert.AreEqual(addresses.Count(), expectedRecipientEmailAddresses.Count, "Different length of addresses");

            for (var i = 0; i < expectedRecipientEmailAddresses.Count; i++)
            {
                var expected = expectedRecipientEmailAddresses[i];
                var actual = addresses[i];

                Assert.AreEqual(expected, actual);
            }

            return smtpMessage;
        }

        /// <summary>
        /// Checks that the specified SmtpMessage has the expected recipient
        /// </summary>
        /// <param name="smtpMessage">The SMTP message.</param>
        /// <param name="expectedRecipientEmailAddress">The expected recipient email address.</param>
        /// <returns></returns>
        public static Mail_Message ShouldBeTo(this Mail_Message smtpMessage, string expectedRecipientEmailAddress)
        {
            var address = smtpMessage.To.OfType<Mail_t_Mailbox>().Select(o => o.Address).Single();

            Assert.AreEqual(address, expectedRecipientEmailAddress, "Missing To email address {0}", expectedRecipientEmailAddress);

            return smtpMessage;
        }
        /// <summary>
        /// Checks that the specified SmtpMessage has the expected subject
        /// </summary>
        /// <param name="smtpMessage">The SMTP message.</param>
        /// <param name="expectedSubject">The expected subject.</param>
        /// <returns></returns>
        public static Mail_Message ShouldHaveSubject(this Mail_Message smtpMessage, string expectedSubject)
        {
            // Confirm that we have the expected subject
            var actualSubject = smtpMessage.Subject;
            Assert.AreEqual(expectedSubject,
                            actualSubject,
                            "Email message subject incorrect");

            return (smtpMessage);
        }

        /// <summary>
        /// Checks that the specified SmtpMessage has a subject that contains the specified string
        /// </summary>
        /// <param name="smtpMessage">The SMTP message.</param>
        /// <param name="expectedSubjectPortion">The expected subject portion.</param>
        /// <returns></returns>
        public static Mail_Message ShouldHaveSubjectThatContains(this Mail_Message smtpMessage, string expectedSubjectPortion)
        {
            // Confirm that we have the expected subject
            var actualSubject = smtpMessage.Header;
            Assert.IsTrue(actualSubject.Contains(expectedSubjectPortion), "Email message subject doesn't contain expected substring");

            return (smtpMessage);
        }

        /// <summary>
        /// Checks that the specified SmtpMessage has the expected body
        /// </summary>
        /// <param name="smtpMessage">The SMTP message.</param>
        /// <param name="expectedBody">The expected body.</param>
        /// <returns></returns>
        public static Mail_Message ShouldHaveBody(this Mail_Message smtpMessage, string expectedBody)
        {
            // Confirm that we have the expected body
            var actualBody = smtpMessage.BodyText;

            // Remove any trailing CRLF on messages.
            if (actualBody.Length > 0)
            {
                actualBody = actualBody.Substring(0, actualBody.LastIndexOf("\r\n"));
            }

            Assert.AreEqual(expectedBody,
                            actualBody,
                            "Email message body incorrect");

            return (smtpMessage);
        }

        /// <summary>
        /// Checks that the specified SmtpMessage has a body that contains the specified string
        /// </summary>
        /// <param name="smtpMessage">The SMTP message.</param>
        /// <param name="expectedBodyPortion">The expected body.</param>
        /// <returns></returns>
        public static Mail_Message ShouldHaveBodyThatContains(this Mail_Message smtpMessage, string expectedBodyPortion)
        {
            // Confirm that we have the expected body substring
            var removed = smtpMessage.BodyText.Replace("=" + System.Environment.NewLine, "");
            Assert.IsTrue(removed.Contains(expectedBodyPortion), "Email message body doesn't contain expected substring");
            return (smtpMessage);
        }

        /// <summary>
        /// Checks the received message 1 attachment.
        /// </summary>
        /// <param name="smtpMessage">The SMTP message.</param>
        /// <returns></returns>
        public static Mail_Message ShouldHaveOneAttachment(this Mail_Message smtpMessage)
        {
            return smtpMessage.ShouldHaveAttachments(1);
        }

        /// <summary>
        /// Checks the received message has the correct number of attachments.
        /// </summary>
        /// <param name="smtpMessage">The SMTP message.</param>
        /// <param name="numberOfAttachments">The number of attachments.</param>
        /// <returns></returns>
        public static Mail_Message ShouldHaveAttachments(this Mail_Message smtpMessage, int numberOfAttachments)
        {
            // Confirm that we have the requested number of emails
            var actualAttachmentCount = smtpMessage.Attachments.Length;
            Assert.AreEqual(numberOfAttachments,
                            actualAttachmentCount,
                            "Email attachment count incorrect");

            return smtpMessage;
        }
    }
}