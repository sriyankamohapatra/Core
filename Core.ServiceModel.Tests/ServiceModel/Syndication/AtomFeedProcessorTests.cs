using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.ServiceModel.Syndication;
using Sfa.Core.ServiceModel.Syndication.Exceptions;
using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace Core.ServiceModel.Tests.ServiceModel.Syndication
{
    [TestClass]
    public class AtomFeedProcessorTests
    {
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithOnePage_MatchingSearchValue()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var lastReadId = "Id1";
                var atomFeedProcessor = new AtomFeedProcessor((uri) => GetFeedLatestPage());

                // Act 
                var actual = new List<SyndicationItem>();
                actual.AddRange(atomFeedProcessor.ReadAndProcess("test", shouldReadAllItems: () => false, isPreviouslyReadItem: item => item.Id == lastReadId, processItem: item => item, afterLastItemProcessed: (item, uri) => { }));

                // Assert
                Assert.AreEqual(1, actual.Count);
                Assert.AreEqual("Id2", actual[0].Id);
                Assert.AreEqual("Item 2", actual[0].Title.Text);
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithOnePage_NonMatchingSearchValue()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var lastReadItemId = "Id5";
                var afterLastItemProcessedCalled = false;
                var atomFeedProcessor = new AtomFeedProcessor((uri) => GetFeedLatestPage());

                // Act 
                var actual = new List<SyndicationItem>();
                actual.AddRange(atomFeedProcessor.ReadAndProcess("test", shouldReadAllItems: () => string.IsNullOrEmpty(lastReadItemId), isPreviouslyReadItem: item => item.Id == lastReadItemId, processItem: item => item, afterLastItemProcessed: (item, uri) => { if (lastReadItemId != string.Empty) { afterLastItemProcessedCalled = true; } }));

                // Assert 
                Assert.AreEqual(true, afterLastItemProcessedCalled);
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithNoItem_SearchValueEmpty()
        {
            using (ShimsContext.Create())
            {
                using (ShimsContext.Create())
                {
                    // Arrange
                    MockLogger();
                    var lastReadItemId = string.Empty;
                    var afterLastItemProcessedCalled = false;
                    var atomFeedProcessor = new AtomFeedProcessor((uri) => GetEmptyPage());

                    // Act 
                    var actual = new List<SyndicationItem>();
                    actual.AddRange(atomFeedProcessor.ReadAndProcess("test", shouldReadAllItems: () => string.IsNullOrEmpty(lastReadItemId), isPreviouslyReadItem: item => item.Id == lastReadItemId, processItem: item => item, afterLastItemProcessed: (item, uri) => { if (lastReadItemId != string.Empty) { afterLastItemProcessedCalled = true; } }));

                    // Assert 
                    Assert.AreEqual(false, afterLastItemProcessedCalled);
                }
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithNoItem_SearchValueNonEmpty()
        {
            using (ShimsContext.Create())
            {
                using (ShimsContext.Create())
                {
                    // Arrange
                    MockLogger();
                    var lastReadItemId = "Id3";
                    var afterLastItemProcessedCalled = false;
                    var atomFeedProcessor = new AtomFeedProcessor((uri) => GetEmptyPage());

                    // Act 
                    var actual = new List<SyndicationItem>();
                    actual.AddRange(atomFeedProcessor.ReadAndProcess("test", shouldReadAllItems: () => string.IsNullOrEmpty(lastReadItemId), isPreviouslyReadItem: item => item.Id == lastReadItemId, processItem: item => item, afterLastItemProcessed: (item, uri) => { if (lastReadItemId != string.Empty) { afterLastItemProcessedCalled = true; } }));

                    // Assert 
                    Assert.AreEqual(true, afterLastItemProcessedCalled);
                }
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithMultiplePage_WithSearchValueAsEmpty()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var lastReadItemId = string.Empty;
                var atomFeedProcessor = new AtomFeedProcessor((uri) =>
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

                actual.AddRange(atomFeedProcessor.ReadAndProcess("http://page0/", shouldReadAllItems: () => String.IsNullOrEmpty(lastReadItemId), isPreviouslyReadItem: item => item.Id == lastReadItemId, processItem: item => item, afterLastItemProcessed: (item, uri) => { }));

                // Assert
                Assert.AreEqual(4, actual.Count);
                Assert.AreEqual("Id3", actual[0].Id);
                Assert.AreEqual("Id4", actual[1].Id);
                Assert.AreEqual("Id1", actual[2].Id);
                Assert.AreEqual("Id2", actual[3].Id);
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
                var lastReadItemId = "Id3";
                var afterLastItemProcessedCalled = false;
                var atomFeedProcessor = new AtomFeedProcessor((uri) =>
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
                actual.AddRange(atomFeedProcessor.ReadAndProcess("http://page0/", shouldReadAllItems: () => String.IsNullOrEmpty(lastReadItemId), isPreviouslyReadItem: item => item.Id == lastReadItemId, processItem: item => item, afterLastItemProcessed: (item, uri) => { afterLastItemProcessedCalled = true; }));

                // Assert
                Assert.AreEqual(3, actual.Count);
                Assert.AreEqual("Id4", actual[0].Id);
                Assert.AreEqual("Id1", actual[1].Id);
                Assert.AreEqual("Id2", actual[2].Id);
                Assert.AreEqual("Item 4", actual[0].Title.Text);
                Assert.AreEqual("Item 1", actual[1].Title.Text);
                Assert.AreEqual("Item 2", actual[2].Title.Text);
                Assert.AreEqual(false, afterLastItemProcessedCalled);
            }
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithEmptyMiddlePage_SearchValueNotEmpty()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var lastReadItemId = "Id3";
                var atomFeedProcessor = new AtomFeedProcessor((uri) =>
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
                actual.AddRange(atomFeedProcessor.ReadAndProcess("http://page0/", shouldReadAllItems: () => String.IsNullOrEmpty(lastReadItemId), isPreviouslyReadItem: item => item.Id == lastReadItemId, processItem: item => item, afterLastItemProcessed: (item, uri) => { }));
            }
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithEmptyMiddlePage_SearchValueEmpty()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var lastReadId = string.Empty;
                var atomFeedProcessor = new AtomFeedProcessor((uri) =>
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
                actual.AddRange(atomFeedProcessor.ReadAndProcess("http://page0/", shouldReadAllItems: () => String.IsNullOrEmpty(lastReadId), isPreviouslyReadItem: item => item.Id == lastReadId, processItem: item => item, afterLastItemProcessed: (item, uri) => { }));
            }
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithEmptyFirstPage_SearchValueNotEmpty()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var lastReadId = "Id3";
                var atomFeedProcessor = new AtomFeedProcessor((uri) =>
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
                actual.AddRange(atomFeedProcessor.ReadAndProcess("http://page0/", shouldReadAllItems: () => String.IsNullOrEmpty(lastReadId), isPreviouslyReadItem: item => item.Id == lastReadId, processItem: item => item, afterLastItemProcessed: (item, uri) => { }));
            }
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithFirstEmptyPage_SearchValueEmpty()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                MockLogger();
                var lastReadId = string.Empty;
                var atomFeedProcessor = new AtomFeedProcessor((uri) =>
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
                actual.AddRange(atomFeedProcessor.ReadAndProcess("http://page0/", shouldReadAllItems: () => String.IsNullOrEmpty(lastReadId), isPreviouslyReadItem: item => item.Id == lastReadId, processItem: item => item, afterLastItemProcessed: (item, uri) => { }));
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
            item1.Id = "Id1";
            item1.Content = SyndicationContent.CreatePlaintextContent("This is the content for Item 1");
            items.Add(item1);

            SyndicationItem item2 = new SyndicationItem();
            item2.Title = new TextSyndicationContent("Item 2");
            item2.Id = "Id2";
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
            item1.Id = "Id3";
            item1.Content = SyndicationContent.CreatePlaintextContent("This is the content for Item 3");
            items.Add(item1);

            SyndicationItem item2 = new SyndicationItem();
            item2.Title = new TextSyndicationContent("Item 4");
            item2.Id = "Id4";
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
