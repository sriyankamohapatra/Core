using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sfa.Core.Context;
using Sfa.Core.Logging;

namespace Sfa.Core
{
    public static class HttpClientExtensions
    {
        public static HttpResponseMessage Get(this HttpClient httpClient, string uri)
        {
            ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => "About to get uri {0}", uri);
            using (var response = Task.Run(() => httpClient.GetAsync(uri)).Result)
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Get call failed with status code {response.StatusCode}, {response.ReasonPhrase}");
                }
                return response;
            }
        }



        public static HttpResponseMessage PostAsXml<T>(this HttpClient httpClient, string uri, T objectToPost)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => "About to post xml to uri {0}", uri);

            using (var response = Task.Run(() => httpClient.PostAsXmlAsync(uri, objectToPost)).Result)
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed for the web call to [{httpClient.BaseAddress}/{uri}] with status code: [{response.StatusCode}], [{response.ReasonPhrase}].");
                }
                return response;
            }
        }


        #region Atom 

        public static SyndicationFeed GetAtomSyndicationFeed(this HttpClient httpClient, string uri, Action<string> afterEachPageLoaded = null)
        {
            var requestHeaders = new[]
            {
                new KeyValuePair<string, string>("Accept", "application/atom+xml"),
            };

            foreach (var header in requestHeaders)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => "About to make call to uri {0}", uri);

            var sw = Stopwatch.StartNew();
            var rawXml = httpClient.GetStringAsync(uri).Result;

            afterEachPageLoaded?.Invoke(uri);

            sw.Stop();
            ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => "Load of feed page {0} took {1}ms", uri, sw.ElapsedMilliseconds);


            var formatter = new Atom10FeedFormatter();
            var doc = XDocument.Parse(rawXml);
            using (var reader = doc.CreateReader())
            {
                formatter.ReadFrom(reader);
            }

            ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => "The load of the feed for uri {0} contained {1} items", uri, formatter.Feed.Items.Count());
            return formatter.Feed;
        }

        #endregion

    }
}
