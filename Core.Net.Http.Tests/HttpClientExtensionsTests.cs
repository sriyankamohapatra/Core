using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Fakes;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Net.Http.Formatting;

namespace Core.Net.Http.Tests
{
    [TestClass]
    public class HttpClientExtensionsTests
    {
        [TestMethod, TestCategory("Unit")]
        public void Get_Success()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();

                // Act
                Sfa.Core.HttpClientExtensions.Get(Get_MockGetClient(successCode: true), "test");
            }
        }

        [ExpectedException(typeof(HttpRequestException))]
        [TestMethod, TestCategory("Unit")]
        public void Get_Error()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();

                // Act
                Sfa.Core.HttpClientExtensions.Get(Get_MockGetClient(successCode: false), "test");
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void GetAtomSyndicationFeed_Success()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var request = new HttpRequestMessage();
                var client = new ShimHttpClient();
                client.GetStringAsyncString = (uri) => Task.FromResult(GetMockSyndiactionFeed());
                client.DefaultRequestHeadersGet = () => request.Headers;

                // Act
                var feed = Sfa.Core.HttpClientExtensions.GetAtomSyndicationFeed(client, "test");

                // Assert
                Assert.AreEqual("Signed Funding Claim Notifications", feed.Title.Text);
                Assert.AreEqual(1, feed.Items.Count());
            }
        }


        #region Private Helpers

        private ShimHttpClient Get_MockGetClient(bool successCode)
        {
            var expectedResponse = new ShimHttpResponseMessage();
            var client = new ShimHttpClient();

            expectedResponse.IsSuccessStatusCodeGet = () => successCode;
            client.GetAsyncString = (uri) => Task.FromResult<HttpResponseMessage>(expectedResponse);
            return client;
        }

        private void MockLogger()
        {
            var logger = new Sfa.Core.Logging.Fakes.StubILogger();
            Sfa.Core.Context.Fakes.ShimApplicationContext.LoggerGet = () => logger;
        }

        private string GetMockSyndiactionFeed()
        {
            return "<feed xmlns=\"http://www.w3.org/2005/Atom\">" +
                   "<title type=\"text\">Signed Funding Claim Notifications</title>" +
                   "<id>uuid:74642edc-ed74-4c6f-9bb6-e6f76563c865;id=2</id>" +
                   "<updated>2016-08-25T10:56:07Z</updated>" +
                   "<author>" +
                   "<name>Funding Claims Management Service</name>" +
                   "</author>" +
                   "<entry>" +
                   "<id>uuid:fdcfcfcd-764a-46a7-b8cf-5b87d15906fa</id>" +
                   "<title type=\"text\"></title>" +
                   "<updated>2016-08-25T11:56:00+01:00</updated>" +
                   "<link rel=\"self\" type=\"application/atom+xml\" href=\"\"/>" +
                   "<content type=\"application/vnd.sfa.fundingclaim.v2+atom+xml\">" +
                   "<FundingClaim>" +
                   "<FundingClaimId>1516-Final_10000981_1</FundingClaimId>" +
                   "<HasBeenSigned>true</HasBeenSigned>" +
                   "</FundingClaim>" +
                   "</content>" +
                   "</entry>" +
                   "</feed>";
        }
        #endregion
    }
}
