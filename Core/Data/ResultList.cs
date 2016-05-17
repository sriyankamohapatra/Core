using System.Collections.Generic;

namespace Sfa.Core.Data
{
    /// <summary>
    /// Default implementation of <see cref="IResultList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public class ResultList<T> : List<T>, IResultList<T>
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
        /// Initializes a new instance of the <see cref="ResultList&lt;T&gt;"/> class.
        /// </summary>
        public ResultList()
        {
            IsPopulated = false;
            IsTruncated = false;
            CurrentPage = 0;
            PageSize = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="list">The list that is wrapped by the new collection.</param>
        /// <exception cref="T:System.ArgumentNullException">list is null.</exception>
        public ResultList(IEnumerable<T> list)
            : base(list)
        {
            IsPopulated = true;
            IsTruncated = false;
            CurrentPage = 0;
            PageSize = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="truncated">if set to <c>true</c> [truncated].</param>
        public ResultList(IEnumerable<T> list, bool truncated)
            : base(list)
        {
            IsPopulated = true;
            IsTruncated = truncated;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="truncated">if set to <c>true</c> [truncated].</param>
        /// <param name="totalNumberOfRecords">the total number of records</param>
        /// <param name="totalNumberOfPages">the total number of pages</param>
        /// <param name="pageSize">The requested page size.</param>
        /// <param name="currentPage">The current page if paging was requested.</param>
        public ResultList(IEnumerable<T> list, bool truncated, int totalNumberOfRecords, int totalNumberOfPages, int pageSize, int currentPage)
            : base(list)
        {
            IsPopulated = true;
            IsTruncated = truncated;
            TotalNumberOfRecords = totalNumberOfRecords;
            TotalNumberOfPages = totalNumberOfPages;
            PageSize = pageSize;
            CurrentPage = currentPage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="obj">The instance to add.</param>
        public ResultList(T obj)
        {
            IsPopulated = true;
            IsTruncated = false;
            Add(obj);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="list">The list</param>
        /// <param name="truncated">if set to <c>true</c> [truncated].</param>
        /// <param name="populated">if set to <c>true</c> [populated].</param>
        public ResultList(IEnumerable<T> list, bool truncated, bool populated)
            : base(list)
        {
            IsPopulated = populated;
            IsTruncated = truncated;
        }

        #endregion
    }
}