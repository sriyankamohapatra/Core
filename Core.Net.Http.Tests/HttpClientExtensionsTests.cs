using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Fakes;
using System.Threading.Tasks;
using System.Linq;
using Sfa.Core.Testing;

namespace Core.Net.Http.Tests
{
    [TestClass]
    public class HttpClientExtensionsTests : BaseTest
    {
        [TestMethod, TestCategory("Unit")]
        public void Get_Success()
        {
            using (ShimsContext.Create())
            {
                // Act
                Sfa.Core.Net.Http.HttpClientExtensions.Get(Get_MockGetClient(successCode: true), "test");
            }
        }

        [ExpectedException(typeof(HttpRequestException))]
        [TestMethod, TestCategory("Unit")]
        public void Get_Error()
        {
            using (ShimsContext.Create())
            {
                // Act
                Sfa.Core.Net.Http.HttpClientExtensions.Get(Get_MockGetClient(successCode: false), "test");
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void GetAtomSyndicationFeed_Success()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var request = new HttpRequestMessage();
                var client = new ShimHttpClient();
                client.GetStringAsyncString = (uri) => Task.FromResult(GetMockSyndiactionFeed());
                client.DefaultRequestHeadersGet = () => request.Headers;

                // Act
                var feed = Sfa.Core.Net.Http.HttpClientExtensions.GetAtomSyndicationFeed(client, "test");

                // Assert
                Assert.AreEqual("Sample Feed Title", feed.Title.Text);
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

        private string GetMockSyndiactionFeed()
        {
            return "<feed xmlns=\"http://www.w3.org/2005/Atom\">" +
                   "<title type=\"text\">Sample Feed Title</title>" +
                   "<id>uuid:00000000-0000-0000-0000-000000000002;id=2</id>" +
                   "<updated>2016-08-25T10:56:07Z</updated>" +
                   "<author>" +
                   "<name>Sample Feed Name</name>" +
                   "</author>" +
                   "<entry>" +
                   "<id>uuid:00000000-0000-0000-0000-000000000002</id>" +
                   "<title type=\"text\"></title>" +
                   "<updated>2016-08-25T11:56:00+01:00</updated>" +
                   "<link rel=\"self\" type=\"application/atom+xml\" href=\"\"/>" +
                   "<content type=\"application/vnd.sfa.fundingclaim.v2+atom+xml\">" +
                   "<Item>" +
                   "<Element1>Element1Value</Element1>" +
                   "<Element2>Element2Value</Element2>" +
                   "</Item>" +
                   "</content>" +
                   "</entry>" +
                   "</feed>";
        }
        #endregion
    }
}
