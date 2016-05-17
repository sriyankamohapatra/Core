using System.Collections.Generic;

namespace Sfa.Core.Data
{
    /// <summary>
    /// Represents the result of a query execution.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public interface IResultList<out T> : IEnumerable<T>, IResultList
    {
    }

    /// <summary>
    /// Represents the result of a query execution.
    /// </summary>
    public interface IResultList
    {
        /// <summary>
        /// Gets a value indicating whether this instance is populated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is populated; otherwise, <c>false</c>.
        /// </value>
        bool IsPopulated { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is truncated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is truncated; otherwise, <c>false</c>.
        /// </value>
        bool IsTruncated { get; }

        /// <summary>
        /// Indicates the total number of records in a paged list
        /// </summary>
        int TotalNumberOfRecords { get; }

        /// <summary>
        /// Indicates the total number of pages in a paged list
        /// </summary>
        int TotalNumberOfPages { get; }

        /// <summary>
        /// The current page in the result list.
        /// </summary>
        int CurrentPage { get; }

        /// <summary>
        /// The request page size.
        /// </summary>
        int PageSize { get; }
    }
}