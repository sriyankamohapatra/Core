using System.ServiceModel.Syndication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.ServiceModel.Syndication;
using System;
using Microsoft.QualityTools.Testing.Fakes;
using System.Collections.Generic;
using Sfa.Core.ServiceModel.Syndication.Exceptions;

namespace Core.ServiceModel.Tests.ServiceModel.Syndication
{
    [TestClass]
    public class GuidItemIdBasedAtomFeedProcessorTests
    {
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithOnePage()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) => GetFeedLatestPage());

                // Act 
                var actual = new List<SyndicationItem>();
                actual.AddRange(guidFeedProcessor.ReadAndProcess("test", new Guid("00000000-0000-0000-0000-000000000001"), item => item));

                // Assert
                Assert.AreEqual(1, actual.Count);
                Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000002", actual[0].Id);
                Assert.AreEqual("Item 2", actual[0].Title.Text);
            }
        }

        [ExpectedException(typeof(GuidBookmarkNotMatchedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithOnePage_NonMatchingGuid()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) => GetFeedLatestPage());

                // Act 
                var actual = new List<SyndicationItem>();
                actual.AddRange(guidFeedProcessor.ReadAndProcess("test", new Guid("00000000-0000-0000-0000-000000000005"), item => item));
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithNoItems_SearchGuidEmpty()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) => GetEmptyPage());

                // Act 
                var actual = new List<SyndicationItem>();
                // This shouldn't throw any exception
                actual.AddRange(guidFeedProcessor.ReadAndProcess("test", new Guid("00000000-0000-0000-0000-000000000000"), item => item));
            }
        }

        [ExpectedException(typeof(GuidBookmarkNotMatchedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithNoItems_SearchGuidNonEmpty()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) => GetEmptyPage());

                // Act 
                var actual = new List<SyndicationItem>();
                actual.AddRange(guidFeedProcessor.ReadAndProcess("test", new Guid("00000000-0000-0000-0000-000000000005"), item => item));
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithMultiplePage_WithSearchGuidAsEmpty()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) =>
                {
                    if (uri == "http://page0/")
                    {
                        return GetFeedLatestPage(prev_url: "http://page1");
                    }
                    if (uri == "http://page1/")
                    {
                        return GetFeedFirstPage(next_url: "http://page0");
                    }
                    return null;
                });

                // Act 
                var actual = new List<SyndicationItem>();
                actual.AddRange(guidFeedProcessor.ReadAndProcess("http://page0/", new Guid("00000000-0000-0000-0000-000000000000"), item => item));

                // Assert
                Assert.AreEqual(4, actual.Count);
                Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000003", actual[0].Id);
                Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000004", actual[1].Id);
                Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000001", actual[2].Id);
                Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000002", actual[3].Id);
                Assert.AreEqual("Item 3", actual[0].Title.Text);
                Assert.AreEqual("Item 4", actual[1].Title.Text);
                Assert.AreEqual("Item 1", actual[2].Title.Text);
                Assert.AreEqual("Item 2", actual[3].Title.Text);
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithMultiplePage_UptoBookmarkId()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) =>
                {
                    if (uri == "http://page0/")
                    {
                        return GetFeedLatestPage(prev_url: "http://page1");
                    }
                    if (uri == "http://page1/")
                    {
                        return GetFeedFirstPage(next_url: "http://page0");
                    }
                    return null;
                });

                // Act 
                var actual = new List<SyndicationItem>();
                actual.AddRange(guidFeedProcessor.ReadAndProcess("http://page0/", new Guid("00000000-0000-0000-0000-000000000003"), item => item));

                // Assert
                Assert.AreEqual(3, actual.Count);
                Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000004", actual[0].Id);
                Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000001", actual[1].Id);
                Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000002", actual[2].Id);
                Assert.AreEqual("Item 4", actual[0].Title.Text);
                Assert.AreEqual("Item 1", actual[1].Title.Text);
                Assert.AreEqual("Item 2", actual[2].Title.Text);
            }
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithEmptyMiddlePage_SearchGuidNotEmpty()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) =>
                {
                    if (uri == "http://page0/")
                    {
                        return GetFeedLatestPage(prev_url: "http://page1/");
                    }
                    if (uri == "http://page1/")
                    {
                        return GetEmptyPage(prev_url: "http://page0/", next_url: "http://page0/");
                    }
                    if (uri == "http://page0/")
                    {
                        return GetFeedFirstPage(next_url: "http://page1");
                    }
                    return null;
                });

                // Act 
                var actual = new List<SyndicationItem>();
                actual.AddRange(guidFeedProcessor.ReadAndProcess("http://page0/", new Guid("00000000-0000-0000-0000-000000000003"), item => item));
            }
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithEmptyMiddlePage_SearchGuidEmpty()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) =>
                {
                    if (uri == "http://page0/")
                    {
                        return GetFeedLatestPage(prev_url: "http://page1/");
                    }
                    if (uri == "http://page1/")
                    {
                        return GetEmptyPage(prev_url: "http://page0/", next_url: "http://page0/");
                    }
                    if (uri == "http://page0/")
                    {
                        return GetFeedFirstPage(next_url: "http://page1");
                    }
                    return null;
                });

                // Act 
                var actual = new List<SyndicationItem>();
                actual.AddRange(guidFeedProcessor.ReadAndProcess("http://page0/", new Guid("00000000-0000-0000-0000-000000000000"), item => item));
            }
        }

        [ExpectedException(typeof(GuidBookmarkNotMatchedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithEmptyFirstPage_SearchGuidNotEmpty()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) =>
                {
                    if (uri == "http://page0/")
                    {
                        return GetFeedLatestPage(prev_url: "http://page1");
                    }
                    if (uri == "http://page1/")
                    {
                        return GetEmptyPage(next_url: "http://page0");
                    }
                    return null;
                });

                // Act 
                var actual = new List<SyndicationItem>();
                actual.AddRange(guidFeedProcessor.ReadAndProcess("http://page0/", new Guid("00000000-0000-0000-0000-000000000005"), item => item));
            }
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithFirstEmptyPage_SearchGuidEmpty()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) =>
                {
                    if (uri == "http://page0/")
                    {
                        return GetFeedLatestPage(prev_url: "http://page1");
                    }
                    if (uri == "http://page1/")
                    {
                        return GetEmptyPage(next_url: "http://page0");
                    }
                    return null;
                });

                // Act 
                var actual = new List<SyndicationItem>();
                actual.AddRange(guidFeedProcessor.ReadAndProcess("http://page0/", new Guid("00000000-0000-0000-0000-000000000000"), item => item));
            }
        }

        #region private Helpers

        private void MockLogger()
        {
            var logger = new Sfa.Core.Logging.Fakes.StubILogger();
            Sfa.Core.Context.Fakes.ShimApplicationContext.LoggerGet = () => logger;
        }


        private SyndicationFeed GetFeedLatestPage(string prev_url = null, string next_url = null)
        {
            var feed = new SyndicationFeed();

            if (prev_url != null)
            {
                feed.Links.Add(new SyndicationLink(new Uri(prev_url), "prev-archive", "prev link", "text/html", 1000));
            }

            if (next_url != null)
            {
                feed.Links.Add(new SyndicationLink(new Uri(next_url), "next-archive", "next link", "text/html", 1000));
            }

            List<SyndicationItem> items = new List<SyndicationItem>();

            SyndicationItem item1 = new SyndicationItem();
            item1.Title = new TextSyndicationContent("Item 1");
            item1.Id = "uuid:00000000-0000-0000-0000-000000000001";
            item1.Content = SyndicationContent.CreatePlaintextContent("This is the content for Item 1");
            items.Add(item1);

            SyndicationItem item2 = new SyndicationItem();
            item2.Title = new TextSyndicationContent("Item 2");
            item2.Id = "uuid:00000000-0000-0000-0000-000000000002";
            item2.Content = SyndicationContent.CreatePlaintextContent("This is the content for Item 2");
            items.Add(item2);

            feed.Items = items;
            return feed;
        }

        private SyndicationFeed GetFeedFirstPage(string prev_url = null, string next_url = null)
        {
            var feed = new SyndicationFeed();

            if (prev_url != null)
            {
                feed.Links.Add(new SyndicationLink(new Uri(prev_url), "prev-archive", "prev link", "text/html", 1000));
            }

            if (next_url != null)
            {
                feed.Links.Add(new SyndicationLink(new Uri(next_url), "next-archive", "next link", "text/html", 1000));
            }
            List<SyndicationItem> items = new List<SyndicationItem>();

            SyndicationItem item1 = new SyndicationItem();
            item1.Title = new TextSyndicationContent("Item 3");
            item1.Id = "uuid:00000000-0000-0000-0000-000000000003";
            item1.Content = SyndicationContent.CreatePlaintextContent("This is the content for Item 3");
            items.Add(item1);

            SyndicationItem item2 = new SyndicationItem();
            item2.Title = new TextSyndicationContent("Item 4");
            item2.Id = "uuid:00000000-0000-0000-0000-000000000004";
            item2.Content = SyndicationContent.CreatePlaintextContent("This is the content for Item 4");
            items.Add(item2);

            feed.Items = items;
            return feed;
        }

        private SyndicationFeed GetEmptyPage(string prev_url = null, string next_url = null)
        {
            var feed = new SyndicationFeed();

            if (prev_url != null)
            {
                feed.Links.Add(new SyndicationLink(new Uri(prev_url), "prev-archive", "prev link", "text/html", 1000));
            }

            if (next_url != null)
            {
                feed.Links.Add(new SyndicationLink(new Uri(next_url), "next-archive", "next link", "text/html", 1000));
            }

            return feed;
        }

        #endregion
    }
}
