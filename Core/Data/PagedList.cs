using System.Collections.Generic;

namespace Sfa.Core.Data
{
    /// <summary>
    /// Default implementation of <see cref="IPagedEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public class PagedList<T> : List<T>, IPagedEnumerable<T>
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is populated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is populated; otherwise, <c>false</c>.
        /// </value>
        public bool IsPopulated { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this instance is truncated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is truncated; otherwise, <c>false</c>.
        /// </value>
        public bool IsTruncated { get; protected set; }

        /// <summary>
        /// Indicates the total number of records in a paged list
        /// </summary>
        public int TotalNumberOfRecords { get; protected set; }

        /// <summary>
        /// Indicates the total number of pages in a paged list
        /// </summary>
        public int TotalNumberOfPages { get; protected set; }

        /// <summary>
        /// The current page in the result list.
        /// </summary>
        public int CurrentPage { get; protected set; }

        /// <summary>
        /// The request page size.
        /// </summary>
        public int PageSize { get; protected set; }

        #endregion


        #region Constructors


        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
        /// </summary>
        public PagedList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
        /// </summary>
        /// <param name="list">The list that is wrapped by the new collection.</param>
        /// <exception cref="T:System.ArgumentNullException">list is null.</exception>
        public PagedList(IEnumerable<T> list)
            : base(list)
        {
            IsPopulated = true;
            TotalNumberOfRecords = Count;
            TotalNumberOfPages = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="totalNumberOfRecords">the total number of records</param>
        /// <param name="totalNumberOfPages">the total number of pages</param>
        /// <param name="pageSize">The requested page size.</param>
        /// <param name="currentPage">The current page if paging was requested.</param>
        public PagedList(IEnumerable<T> list, int totalNumberOfRecords, int totalNumberOfPages, int pageSize, int currentPage)
            : base(list)
        {
            IsPopulated = true;
            IsTruncated = Count < totalNumberOfRecords;
            TotalNumberOfRecords = totalNumberOfRecords;
            TotalNumberOfPages = totalNumberOfPages;
            PageSize = pageSize;
            CurrentPage = currentPage;
        }

        #endregion
    }
}