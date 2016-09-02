using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using Sfa.Core.ServiceModel.Syndication.Exceptions;
using Sfa.Core.Context;
using Sfa.Core.Logging;

namespace Sfa.Core.ServiceModel.Syndication
{
    /// <summary>
    /// Helper utility for processing items in the content of an Atom Feed reader
    /// </summary>
    public class AtomFeedProcessor
    {
        private readonly Func<string, SyndicationFeed> _read;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="read">A function which takes the current atom feed page and returns the SyndicationFeed for this page.</param>
        public AtomFeedProcessor(Func<string, SyndicationFeed> read)
        {
            _read = read;
        }

        /// <summary>
        /// Reads the feed from the place last read, which is defined by <paramref name="isPreviouslyReadItem"/> and returns the content processed by <paramref name="processItem"/>.
        /// </summary>
        /// <typeparam name="T">The type of item that is expected to be in the content of each item within the feed once its been processed.</typeparam>
        /// <param name="initialUrl">The initial page to read for the feed.</param>
        /// <param name="shouldReadAllItems">Flag indicating is all items of the atom feed should be processed and returned from the beginning.</param>
        /// <param name="isPreviouslyReadItem">Determines if the item passed has already been read. If <c>true</c> then only items that occur after this in the feed will be processed and returned.</param>
        /// <param name="processItem">Defines a function which takes each <see cref="SyndicationItem"/> and returns the desired item.</param>
        /// <param name="afterLastItemProcessed">Triggered when all items on the feed have been read and no previous item has been found.</param>
        /// <param name="lastItemProcessed">Fired when all processing of items has been completed. Contains the last item that was processed.</param>
        /// <returns>An enumerable list of processed items from a feed starting with the first item after a previous item match, if such a match exists.</returns>
        public IEnumerable<T> ReadAndProcess<T>(string initialUrl, Func<bool> shouldReadAllItems, Func<SyndicationItem, bool> isPreviouslyReadItem, Func<SyndicationItem, T> processItem, Action<SyndicationItem, string> afterLastItemProcessed, Action<SyndicationItem> lastItemProcessed = null)
        {
            var startingUrlToProcess = FindStartingUrlToProcess(initialUrl, afterLastItemProcessed, isPreviouslyReadItem);
            return ReadAndProcessItemsFromUrls(startingUrlToProcess, shouldReadAllItems, isPreviouslyReadItem, processItem, lastItemProcessed);
        }


        #region Processing 

        private string FindStartingUrlToProcess(string initialUrl, Action<SyndicationItem, string> afterLastItemProcessed, Func<SyndicationItem, bool> isPreviouslyReadItem)
        {
            var currentUri = initialUrl;
            var previousUri = string.Empty;
            SyndicationItem lastItemRead = null;

            while (currentUri != null)
            {
                var feed = _read(currentUri);

                foreach (var item in feed.Items)
                {
                    lastItemRead = item;
                    if (isPreviouslyReadItem(item))
                    {
                        ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => $"Matched the item to item with id {item.Id}. Stopping loading any more pages or processing further items from this atom page.");
                        return currentUri;
                    }
                    ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => $"Unmatched item with event id {item.Id}.");
                }

                previousUri = currentUri;
                var link = feed.Links.SingleOrDefault(l => l.RelationshipType == "prev-archive");
                currentUri = link?.Uri.ToString();

                // If we're not the first page and its empty, there is a problem with the feed
                if (!feed.Items.Any() && link != null)
                {
                    throw new EmptyPageOnFeedException(previousUri);
                }
            }

            afterLastItemProcessed(lastItemRead, previousUri);

            return previousUri;
        }


        private IEnumerable<T> ReadAndProcessItemsFromUrls<T>(string startingUrlToReadFrom, Func<bool> shouldReadAllItems, Func<SyndicationItem, bool> isPreviouslyReadItem, Func<SyndicationItem, T> processItem, Action<SyndicationItem> lastItemProcessed = null)
        {
            var startProcessing = shouldReadAllItems();
            var currentUri = startingUrlToReadFrom;
            SyndicationItem lastItem = null;

            while (!string.IsNullOrWhiteSpace(currentUri))
            {
                var feed = _read(currentUri);

                foreach (var item in feed.Items)
                {
                    lastItem = item;

                    // We have found the original bookmark, therefore we can start processing on the next item
                    if (!startProcessing && isPreviouslyReadItem(item))
                    {
                        ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => $"Matched the item with id {item.Id} on second read. Now start to processing the next items.");
                        startProcessing = true;
                    }
                    else if (startProcessing)
                    {
                        ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => $"Process item with Id: {item.Id} on second pass.");

                        yield return processItem(item);
                    }
                }

                
                var previousLink = feed.Links.SingleOrDefault(l => l.RelationshipType == "prev-archive");
                var nextLink = feed.Links.SingleOrDefault(l => l.RelationshipType == "next-archive");
                currentUri = nextLink?.Uri.ToString();

                // If we're not the first page and its empty, there is a problem with the feed
                if (!feed.Items.Any() && nextLink != null )
                {
                    throw new EmptyPageOnFeedException(currentUri);
                }
            }

            lastItemProcessed?.Invoke(lastItem);
        }

        #endregion
    }
}