using System.ServiceModel.Syndication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.ServiceModel.Syndication;
using System;
using System.Collections.Generic;
using Sfa.Core.ServiceModel.Syndication.Exceptions;
using Sfa.Core.Testing;

namespace Core.ServiceModel.Tests.ServiceModel.Syndication
{
    [TestClass]
    public class GuidItemIdBasedAtomFeedProcessorTests : BaseTest
    {
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithOnePage_MatchingGuidFound_ElementsAfterMatchingGuidRead()
        {
            // Arrange
            var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) => GetFeedPage0());

            // Act 
            var actual = new List<SyndicationItem>();
            actual.AddRange(guidFeedProcessor.ReadAndProcess("test", new Guid("00000000-0000-0000-0000-000000000001"), item => item));

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000002", actual[0].Id);
            Assert.AreEqual("Item 2", actual[0].Title.Text);
        }

        [ExpectedException(typeof(GuidBookmarkNotMatchedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithOnePage_NoMatchesFound_GuidBookmarkNotMatchedExceptionThrown()
        {
            // Arrange
            var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) => GetFeedPage0());

            // Act 
            guidFeedProcessor.ReadAndProcess("test", new Guid("00000000-0000-0000-0000-000000000005"), item => item);
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithNoItems_SearchGuidEmpty_NoExceptionThrown()
        {
            // Arrange
            var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) => GetEmptyPage());

            // Act 
            var actual = new List<SyndicationItem>();

            // This shouldn't throw any exception
            guidFeedProcessor.ReadAndProcess("test", new Guid("00000000-0000-0000-0000-000000000000"), item => item);
        }

        [ExpectedException(typeof(GuidBookmarkNotMatchedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithNoItems_SearchGuidNonEmpty_GuidBookmarkNotMatchedExceptionThrown()
        {
            // Arrange
            var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) => GetEmptyPage());

            // Act 
            guidFeedProcessor.ReadAndProcess("test", new Guid("00000000-0000-0000-0000-000000000005"), item => item);
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithMultiplePages_WithSearchGuidAsEmpty_ShouldReadAllItems()
        {
            // Arrange
            var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) =>
            {
                if (uri == "http://page0/")
                {
                    return GetFeedPage0(next_url: "http://page1");
                }
                if (uri == "http://page1/")
                {
                    return GetFeedPage1(prev_url: "http://page0");
                }
                return null;
            });

            // Act 
            var actual = new List<SyndicationItem>();
            actual.AddRange(guidFeedProcessor.ReadAndProcess("http://page1/", new Guid("00000000-0000-0000-0000-000000000000"), item => item));

            // Assert
            Assert.AreEqual(4, actual.Count);
            Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000001", actual[0].Id);
            Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000002", actual[1].Id);
            Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000003", actual[2].Id);
            Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000004", actual[3].Id);
            Assert.AreEqual("Item 1", actual[0].Title.Text);
            Assert.AreEqual("Item 2", actual[1].Title.Text);
            Assert.AreEqual("Item 3", actual[2].Title.Text);
            Assert.AreEqual("Item 4", actual[3].Title.Text);
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithMultiplePages_WithMatchingGuidOnPage0_ElementsAfterMatchingGuidRead()
        {
            // Arrange
            var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) =>
            {
                if (uri == "http://page0/")
                {
                    return GetFeedPage0(next_url: "http://page1");
                }
                if (uri == "http://page1/")
                {
                    return GetFeedPage1(prev_url: "http://page0");
                }
                return null;
            });

            // Act 
            var actual = new List<SyndicationItem>();
            actual.AddRange(guidFeedProcessor.ReadAndProcess("http://page1/", new Guid("00000000-0000-0000-0000-000000000001"), item => item));

            // Assert
            Assert.AreEqual(3, actual.Count);
            Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000002", actual[0].Id);
            Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000003", actual[1].Id);
            Assert.AreEqual("uuid:00000000-0000-0000-0000-000000000004", actual[2].Id);
            Assert.AreEqual("Item 2", actual[0].Title.Text);
            Assert.AreEqual("Item 3", actual[1].Title.Text);
            Assert.AreEqual("Item 4", actual[2].Title.Text);
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithThreePages_WithEmptyPage1_SearchGuidNotEmpty_EmptyPageOnFeedExceptionThrown()
        {
            // Arrange
            var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) =>
            {
                if (uri == "http://page0/")
                {
                    return GetFeedPage0(next_url: "http://page1/");
                }
                if (uri == "http://page1/")
                {
                    return GetEmptyPage(prev_url: "http://page0/", next_url: "http://page2/");
                }
                if (uri == "http://page2/")
                {
                    return GetFeedPage1(prev_url: "http://page1");
                }
                return null;
            });

            // Act 
            guidFeedProcessor.ReadAndProcess("http://page2/", new Guid("00000000-0000-0000-0000-000000000001"), item => item);
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithThreePages_WithPage1Empty_SearchGuidEmpty_EmptyPageOnFeedExceptionThrown()
        {
            // Arrange
            var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) =>
            {
                if (uri == "http://page0/")
                {
                    return GetFeedPage0(next_url: "http://page1/");
                }
                if (uri == "http://page1/")
                {
                    return GetEmptyPage(prev_url: "http://page0/", next_url: "http://page2/");
                }
                if (uri == "http://page2/")
                {
                    return GetFeedPage1(prev_url: "http://page1");
                }
                return null;
            });

            // Act 
            var actual = new List<SyndicationItem>();
            actual.AddRange(guidFeedProcessor.ReadAndProcess("http://page2/", new Guid("00000000-0000-0000-0000-000000000000"), item => item));
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithEmptyPage0_SearchGuidNotEmpty_GuidBookmarkNotMatchedExceptionThrown()
        {
            // Arrange
            var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) =>
            {
                if (uri == "http://page0/")
                {
                    return GetEmptyPage(next_url: "http://page1");
                }
                if (uri == "http://page1/")
                {
                    return GetFeedPage1(prev_url: "http://page0");
                }
                return null;
            });

            // Act 
            guidFeedProcessor.ReadAndProcess("http://page1/", new Guid("00000000-0000-0000-0000-000000000005"), item => item);
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithEmptyPage0_SearchGuidEmpty_EmptyPageOnFeedExceptionThrown()
        {
            // Arrange
            var guidFeedProcessor = new GuidItemIdBasedAtomFeedProcessor((uri) =>
            {
                if (uri == "http://page0/")
                {
                    return GetEmptyPage(next_url: "http://page1");
                }
                if (uri == "http://page1/")
                {
                    return GetFeedPage1(prev_url: "http://page0");
                }
                return null;
            });

            // Act 
            var actual = new List<SyndicationItem>();
            actual.AddRange(guidFeedProcessor.ReadAndProcess("http://page1/", new Guid("00000000-0000-0000-0000-000000000000"), item => item));
        }

        #region private Helpers

        private void MockLogger()
        {
            var logger = new Sfa.Core.Logging.Fakes.StubILogger();
            Sfa.Core.Context.Fakes.ShimApplicationContext.LoggerGet = () => logger;
        }


        private SyndicationFeed GetFeedPage0(string prev_url = null, string next_url = null)
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

        private SyndicationFeed GetFeedPage1(string prev_url = null, string next_url = null)
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
