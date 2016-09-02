using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfa.Core.ServiceModel.Syndication;
using Sfa.Core.ServiceModel.Syndication.Exceptions;
using Sfa.Core.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;

namespace Core.ServiceModel.Tests.ServiceModel.Syndication
{
    [TestClass]
    public class AtomFeedProcessorTests : BaseTest
    {
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithOnePage_MatchingSearchValueFound_ElementsAfterMatchingSearchValueRead()
        {
            // Arrange
            var lastReadId = "Id1";
            var afterLastItemProcessedCalled = false;
            var atomFeedProcessor = new AtomFeedProcessor((uri) => GetFeedPage0());

            // Act 
            var actual = atomFeedProcessor.ReadAndProcess("test", 
                shouldReadAllItems: () => false, 
                isPreviouslyReadItem: item => item.Id == lastReadId, 
                processItem: item => item, 
                afterLastItemProcessed: (item, uri) => { afterLastItemProcessedCalled = true; }).ToList();

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Id2", actual[0].Id);
            Assert.AreEqual("Item 2", actual[0].Title.Text);
            Assert.AreEqual(false, afterLastItemProcessedCalled);
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithOneNonEmptyPage_ShouldNotReadAllItems_AfterLastItemProcessedCalled()
        {
            // Arrange
            var afterLastItemProcessedCalled = false;
            var atomFeedProcessor = new AtomFeedProcessor((uri) => GetFeedPage1());

            // Act 
            atomFeedProcessor.ReadAndProcess(
                "test", 
                shouldReadAllItems: () => false, 
                isPreviouslyReadItem: item => false, 
                processItem: item => item, 
                afterLastItemProcessed: (item, uri) => {  afterLastItemProcessedCalled = true; });

            // Assert 
            Assert.AreEqual(true, afterLastItemProcessedCalled);
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithNoItems_ShouldReadAllItems_AfterLastItemProcessedCalled()
        {
            // Arrange
            var afterLastItemProcessedCalled = false;
            var atomFeedProcessor = new AtomFeedProcessor((uri) => GetEmptyPage());

            // Act 
            atomFeedProcessor.ReadAndProcess(
               "test", 
               shouldReadAllItems: () => true, 
               isPreviouslyReadItem: item => false,
               processItem: item => item, 
               afterLastItemProcessed: (item, uri) => { afterLastItemProcessedCalled = true; });

            // Assert 
            Assert.AreEqual(true, afterLastItemProcessedCalled);
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithNoItems_ShouldNotReadAllItems_AfterLastItemProcessedCalled()
        {
            // Arrange
            var afterLastItemProcessedCalled = false;
            var atomFeedProcessor = new AtomFeedProcessor((uri) => GetEmptyPage());

            // Act 
            atomFeedProcessor.ReadAndProcess(
                "test", 
                 shouldReadAllItems: () => false, 
                 isPreviouslyReadItem: item => false, 
                 processItem: item => item, 
                 afterLastItemProcessed: (item, uri) => {  afterLastItemProcessedCalled = true; });

            // Assert 
            Assert.AreEqual(true, afterLastItemProcessedCalled);
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithMultiplePages_SearchValueAsEmpty_AllItemsRetreived()
        {
            // Arrange
            var lastReadItemId = string.Empty;
            var afterLastItemProcessedCalled = false;
            var atomFeedProcessor = new AtomFeedProcessor((uri) =>
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

            actual.AddRange(atomFeedProcessor.ReadAndProcess(
                "http://page0/", 
                shouldReadAllItems: () => true, 
                isPreviouslyReadItem: item => item.Id == lastReadItemId, 
                processItem: item => item, 
                afterLastItemProcessed: (item, uri) => { afterLastItemProcessedCalled = true; }));

            // Assert
            Assert.AreEqual(4, actual.Count);
            Assert.AreEqual("Id1", actual[0].Id);
            Assert.AreEqual("Id2", actual[1].Id);
            Assert.AreEqual("Id3", actual[2].Id);
            Assert.AreEqual("Id4", actual[3].Id);
            Assert.AreEqual("Item 1", actual[0].Title.Text);
            Assert.AreEqual("Item 2", actual[1].Title.Text);
            Assert.AreEqual("Item 3", actual[2].Title.Text);
            Assert.AreEqual("Item 4", actual[3].Title.Text);
            Assert.AreEqual(true, afterLastItemProcessedCalled);
        }

        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithMultiplePages_MatchingSearchValueFound_ElementsAfterMatchingSearchValueRead()
        {

            // Arrange
            var lastReadItemId = "Id1";
            var afterLastItemProcessedCalled = false;
            var atomFeedProcessor = new AtomFeedProcessor((uri) =>
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
            actual.AddRange(atomFeedProcessor.ReadAndProcess(
                "http://page1/", 
                shouldReadAllItems: () => false, 
                isPreviouslyReadItem: item => item.Id == lastReadItemId, 
                processItem: item => item, 
                afterLastItemProcessed: (item, uri) => { afterLastItemProcessedCalled = true; }));

            // Assert
            Assert.AreEqual(3, actual.Count);
            Assert.AreEqual("Id2", actual[0].Id);
            Assert.AreEqual("Id3", actual[1].Id);
            Assert.AreEqual("Id4", actual[2].Id);
            Assert.AreEqual("Item 2", actual[0].Title.Text);
            Assert.AreEqual("Item 3", actual[1].Title.Text);
            Assert.AreEqual("Item 4", actual[2].Title.Text);
            Assert.AreEqual(false, afterLastItemProcessedCalled);
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithEmptyPage1_ShouldNotReadAllItems_EmptyPageOnFeedExceptionThrown()
        {
            // Arrange
            
            var atomFeedProcessor = new AtomFeedProcessor((uri) =>
            {
                if (uri == "http://page0/")
                {
                    return GetFeedPage0(next_url: "http://page1/");
                }
                if (uri == "http://page1/")
                {
                    return GetEmptyPage(prev_url: "http://page0/", next_url: "http://page1/");
                }
                if (uri == "http://page2/")
                {
                    return GetFeedPage0(prev_url: "http://page1");
                }
                return null;
            });

            // Act 
            atomFeedProcessor.ReadAndProcess(
                "http://page2/", 
                 shouldReadAllItems: () => false, 
                 isPreviouslyReadItem: item => false, 
                 processItem: item => item, 
                 afterLastItemProcessed: (item, uri) => { });
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithEmptyPage1_ShouldReadAllItems_EmptyPageOnFeedExceptionThrown()
        {

            // Arrange
            var atomFeedProcessor = new AtomFeedProcessor((uri) =>
            {
                if (uri == "http://page0/")
                {
                    return GetFeedPage0(next_url: "http://page1/");
                }
                if (uri == "http://page1/")
                {
                    return GetEmptyPage(prev_url: "http://page0/", next_url: "http://page1/");
                }
                if (uri == "http://page2/")
                {
                    return GetFeedPage0(prev_url: "http://page1");
                }
                return null;
            }); ;

            // Act 
            atomFeedProcessor.ReadAndProcess(
                "http://page2/",
                shouldReadAllItems: () => true, 
                isPreviouslyReadItem: item => false, 
                processItem: item => item, 
                afterLastItemProcessed: (item, uri) => { });
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithEmptyPage0_ShouldNotReadAllItems_EmptyPageOnFeedExceptionThrown()
        {
            // Arrange
            
            var atomFeedProcessor = new AtomFeedProcessor((uri) =>
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
            atomFeedProcessor.ReadAndProcess(
                 "http://page1/",
                 shouldReadAllItems: () => false,
                 isPreviouslyReadItem: item => false,
                 processItem: item => item, 
                 afterLastItemProcessed: (item, uri) => { });
        }

        [ExpectedException(typeof(EmptyPageOnFeedException))]
        [TestMethod, TestCategory("Unit")]
        public void ReadAndProcess_FeedWithEmptyPage0_ShouldReadAllItems_EmptyPageOnFeedExceptionThrown()
        {

            // Arrange
            var atomFeedProcessor = new AtomFeedProcessor((uri) =>
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
            atomFeedProcessor.ReadAndProcess(
                "http://page1/", 
                 shouldReadAllItems: () => true, 
                 isPreviouslyReadItem: item => false, 
                 processItem: item => item, 
                 afterLastItemProcessed: (item, uri) => { });
        }

        #region private Helpers

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
