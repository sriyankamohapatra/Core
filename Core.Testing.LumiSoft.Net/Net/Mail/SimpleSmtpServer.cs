using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using LumiSoft.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.Context;
using Sfa.Core.Logging;

namespace Sfa.Core.Net.Mail
{
    /// <summary>
    /// SimpleSmtpServer used for testing emails.
    /// Used in combination with the .eml reader from LumiSoft
    /// </summary>
    public class SimpleSmtpServer
    {
        #region Constants

        private const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        #endregion


        /// <summary>
        /// Gets the fluent dummy instance.
        /// </summary>
        /// <value>The fluent dummy instance.</value>
        private static SimpleSmtpServer FluentDummyInstance => null;

        /// <summary>
        /// Starts this instance running.
        /// </summary>
        /// <returns></returns>
        public static SimpleSmtpServer Start()
        {
            DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            SetRelativePickupDirectoryLocation("Mail\\");

            return FluentDummyInstance;
        }

        /// <summary>
        /// Stops this instance and cleans up the emails.
        /// </summary>
        /// <returns></returns>
        public static SimpleSmtpServer Stop()
        {
            DiscardAllEmails();
            Directory.Delete(PickupDirectoryLocation);

            return FluentDummyInstance;
        }

        public static SimpleSmtpServer EnsureAllEmailsPickedUp()
        {
            // Track those messages in the Email Test Server that the test didn't "handle"
            var unhandledEmailsAtTestCompletion = Emails?.Count();

            // Now clear the emails ready for the next test.
            DiscardAllEmails();


            // Last thing is to fail if there were unprocessed messages
            // Done last to ensure the tear downs have happened and 
            // the following tests start with a clean slate.
            if (unhandledEmailsAtTestCompletion != 0)
            {
                Assert.Fail("{0} unexpected Email Messages at service test completion.\nAdd tests for emails and call 'EmailTestServer.DiscardAllEmails()' at end of service test.", unhandledEmailsAtTestCompletion);
            }

            return FluentDummyInstance;
        }

        #region Setup

        /// <summary>
        /// Gets the current emails.
        /// </summary>
        /// <value>The emails.</value>
        public static IEnumerable<Mail_Message> Emails
        {
            get
            {
                IEnumerable<Mail_Message> emails = null;
                var sw = Stopwatch.StartNew();
                while (emails == null && (sw.ElapsedMilliseconds < TimeSpan.FromSeconds(10).TotalMilliseconds))
                {
                    try
                    {
                        emails = Directory.GetFiles(PickupDirectoryLocation).Select(Mail_Message.ParseFromFile).OrderBy(mm => mm.Date).ToList();
                    }
                    catch (Exception)
                    {
                    }
                }
                return emails;
            }
        }

        /// <summary>
        /// Gets or sets the delivery method.
        /// </summary>
        /// <value>The delivery method.</value>
        public static SmtpDeliveryMethod DeliveryMethod
        {
            get
            {
                return (SmtpDeliveryMethod)DeliveryMethodField.GetValue(SmtpSection);
            }
            set
            {
                DeliveryMethodField.SetValue(SmtpSection, value);
            }
        }

        /// <summary>
        /// Sets the relative pickup directory location. Relative to the running directory
        /// </summary>
        /// <param name="path">The path.</param>
        public static void SetRelativePickupDirectoryLocation(string path)
        {
            SetPickupDirectoryLocation(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        /// <summary>
        /// Sets the pickup directory location combining two paths.
        /// </summary>
        /// <param name="path1">The first path of the path.</param>
        /// <param name="path2">The second part of the path.</param>
        public static void SetPickupDirectoryLocation(string path1, string path2)
        {
            PickupDirectoryLocation = Path.Combine(path1, path2);
        }

        /// <summary>
        /// Gets or sets the pickup directory location.
        /// This must be an absolute directory path.
        /// </summary>
        /// <value>The pickup directory location.</value>
        public static string PickupDirectoryLocation
        {
            get { return (string)PickupDirectoryLocationField.GetValue(SpecifiedPickupDirectory); }
            set
            {
                if (!Directory.Exists(value))
                {
                    Console.WriteLine("Creating directory {0}", value);
                    Directory.CreateDirectory(value);
                }
                else
                {
                    Console.WriteLine("using email drop box {0}", value);
                    // Could have a stopped test, so the directory might exist, but we need to clean it out, just in case
                    foreach (var file in Directory.GetFiles(value))
                    {
                        File.Delete(file);
                    }
                }

                PickupDirectoryLocationField.SetValue(SpecifiedPickupDirectory, value);
            }
        }

        #endregion

        #region Internals

        private static FieldInfo DeliveryMethodField => SmtpSection.GetType().GetField("deliveryMethod", InstanceFlags);

        private static FieldInfo PickupDirectoryLocationField => SpecifiedPickupDirectory.GetType().GetField("pickupDirectoryLocation", InstanceFlags);

        private static object SmtpSection
        {
            get
            {
                var property = typeof(SmtpClient).GetProperty("MailConfiguration", BindingFlags.Static | BindingFlags.NonPublic);
                var mailConfiguration = property.GetValue(null, null);

                property = mailConfiguration.GetType().GetProperty("Smtp", InstanceFlags);
                return property.GetValue(mailConfiguration, null);
            }
        }

        private static object SpecifiedPickupDirectory
        {
            get
            {
                var property = SmtpSection.GetType().GetProperty("SpecifiedPickupDirectory", InstanceFlags);
                return property.GetValue(SmtpSection, null);
            }
        }

        #endregion


        /// <summary>
        /// Discards any emails that the specified SimpleSmtpServer holds
        /// </summary>
        public static SimpleSmtpServer DiscardAllEmails()
        {
            // Discard the emails that exist
            if (string.IsNullOrWhiteSpace(PickupDirectoryLocation))
            {
                var message = $"The {nameof(PickupDirectoryLocation)} has not been specified. Have you setup the Server? Put in AssmeblyInitilize to save resource usage";
                ApplicationContext.Logger.Log(LoggingLevel.Error, CoreLoggingCategory.Diagnostics, () => message);
                Assert.Fail(message);
            }
            else
            {
                Directory.GetFiles(PickupDirectoryLocation).ToList().ForEach(File.Delete);
            }

            return FluentDummyInstance;
        }

        /// <summary>
        /// Checks that the specified SimpleSmtpServer holds no emails
        /// </summary>
        public static SimpleSmtpServer ShouldHaveNoEmails()
        {
            // Confirm that we have no emails
            ShouldHaveEmails(0);

            return (FluentDummyInstance);
        }

        /// <summary>
        /// Checks that the specified SimpleSmtpServer holds one email
        /// </summary>
        public static SimpleSmtpServer ShouldHaveOneEmail()
        {
            // Confirm that we have one email
            ShouldHaveEmails(1);

            return (FluentDummyInstance);
        }

        /// <summary>
        /// Checks that the specified SimpleSmtpServer holds two email
        /// </summary>
        public static SimpleSmtpServer ShouldHaveTwoEmails()
        {
            // Confirm that we have two emails
            ShouldHaveEmails(2);

            return (FluentDummyInstance);
        }

        /// <summary>
        /// Checks that the specified SimpleSmtpServer holds two email
        /// </summary>
        public static SimpleSmtpServer ShouldHaveThreeEmails()
        {
            // Confirm that we have two emails
            ShouldHaveEmails(3);

            return (FluentDummyInstance);
        }

        /// <summary>
        /// Checks that the specified SimpleSmtpServer holds the specified number of emails
        /// </summary>
        /// <param name="expectedEmailCount">The expected email count.</param>
        public static SimpleSmtpServer ShouldHaveEmails(int expectedEmailCount)
        {
            // Confirm that we have the requested number of emails
            var sw = Stopwatch.StartNew();

            var matched = false;
            while (sw.ElapsedMilliseconds < 10000)
            {
                try
                {
                    var emailCount = Directory.GetFiles(PickupDirectoryLocation).Length;
                    ApplicationContext.Logger.Log(LoggingLevel.Error, CoreLoggingCategory.Diagnostics, () => "Email count at {0} is {1}. Expect {2}", DateTime.Now, emailCount, expectedEmailCount);

                    if (expectedEmailCount == emailCount)
                    {
                        matched = true;
                        break;
                    }
                }
                catch
                {
                }
                Thread.Sleep(250);
            }

            Assert.IsTrue(matched, "Email Test Server received email count incorrect");

            return FluentDummyInstance;
        }

        /// <summary>
        /// Gets the first email received from the specified simple SMTP server.
        /// </summary>
        /// <returns>An SmtpMessage</returns>
        public static Mail_Message FirstEmail()
        {
            return (Emails.FirstOrDefault());
        }

        /// <summary>
        /// Gets the second email received from the specified simple SMTP server.
        /// </summary>
        /// <returns>An SmtpMessage</returns>
        public static Mail_Message SecondEmail()
        {
            return (Emails.ToList()[1]);
        }

        /// <summary>
        /// Gets the third email received from the specified simple SMTP server.
        /// </summary>
        /// <returns>An SmtpMessage</returns>
        public static Mail_Message ThirdEmail()
        {
            return (Emails.ToList()[2]);
        }

        /// <summary>
        /// Gets the specified emails from the specified simple SMTP server.
        /// </summary>
        /// <param name="count">The (one based) count of the email to retrieve.</param>
        /// <returns>An SmtpMessage</returns>
        public static Mail_Message Email(int count)
        {
            return (Emails.ToList()[count - 1]);
        }
    }
}