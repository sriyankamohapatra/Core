using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using Sfa.Core.ServiceModel.Syndication.Exceptions;


namespace Sfa.Core.ServiceModel.Syndication
{
    /// <summary>
    /// Atom feed processed where the item id is a <see cref="Guid"/> and is used as the way to determine the last item read from the feed.
    /// </summary>
    public class GuidItemIdBasedAtomFeedProcessor : AtomFeedProcessor
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="read">A function which takes the current atom feed page and returns the SyndicationFeed for this page.</param>
        public GuidItemIdBasedAtomFeedProcessor(Func<string, SyndicationFeed> read)
            : base(read)
        {
        }


        /// <summary>
        /// Reads the feed from the place last read and returns the content processed by <paramref name="processItem"/>.
        /// </summary>
        /// <typeparam name="T">The type of item that is expected to be in the content of each item within the feed once its been processed.</typeparam>
        /// <param name="initialUrl">The initial page to read for the feed.</param>
        /// <param name="lastReadItemId">The last Item Id read by a previous read of the feed.</param>
        /// <param name="processItem">Defines a function which takes each <see cref="SyndicationItem"/> and returns the desired item.</param>
        /// <param name="storeLatestReadItemId">A call back containing the last Item Id read by the reader. The client should store this Id and pass in on the next call as <paramref name="lastReadItemId"/>.</param>
        /// <returns>An enumerable list of processed items from a feed starting with the first item after a previous item match, if such a match exists.</returns>
        public IEnumerable<T> ReadAndProcess<T>(string initialUrl, Guid lastReadItemId, Func<SyndicationItem, T> processItem, Action<Guid> storeLatestReadItemId = null)
        {
            Func<bool> shouldReadAllItems = () => lastReadItemId == Guid.Empty;

            Func<SyndicationItem, bool> isPreviouslyReadItem = item =>
            {
                var eventId = item.IdAsGuid();

                // We have found the original bookmark, therefore we don't want to process this contract or any before it
                return eventId == lastReadItemId;
            };

            Action<SyndicationItem, string> afterLastItemProcessed = (item, uri) =>
            {
                // We're now on the first page of the feed (not the first read page) as the uri is null
                if (lastReadItemId != Guid.Empty)
                {
                    // This means that we have expected to have matched an event somewhere but we haven't
                    throw new GuidBookmarkNotMatchedException(lastReadItemId, uri);
                }
            };

            Action<SyndicationItem> lastItemProcessed = item => { storeLatestReadItemId?.Invoke(item?.IdAsGuid() ?? lastReadItemId); };

            return ReadAndProcess(initialUrl, shouldReadAllItems, isPreviouslyReadItem, processItem, afterLastItemProcessed, lastItemProcessed);
        }
    }
}