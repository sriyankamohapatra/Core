using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sfa.Core.Exceptions;

namespace Sfa.Core.Data
{
    /// <summary>
    /// Base class for data queries.
    /// </summary>
    /// <typeparam name="TReturn">The type of the instance being returned from the query.</typeparam>
    /// <typeparam name="TQuery">The type of the instance being queried against.</typeparam>
    public abstract class Query<TReturn, TQuery> : IQuery<TReturn>
    {
        #region Properties

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; protected set; }

        /// <summary>
        /// Get the page number to query
        /// </summary>
        public int PageNumber { get; protected set; }

        /// <summary>
        /// Gets the actual size of the page.
        /// </summary>
        /// <value>The actual size of the page.</value>
        protected int ActualPageSize => PageSize + 1;

        /// <summary>
        /// Gets a value indicating whether [should be truncated].
        /// </summary>
        /// <value><c>true</c> if [should be truncated]; otherwise, <c>false</c>.</value>
        protected bool ShouldBeTruncated => PageSize > 0;

        #endregion


        #region Contexts

        /// <summary>
        /// Gets the data mapper.
        /// </summary>
        protected abstract IQueryable<TQuery> Queryable { get; }

        #endregion


        #region Execute query

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns>The results of executing the query.</returns>
        public virtual IResultList<TReturn> GetResultList(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;

            return GetResultList(Queryable);
        }

        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        /// <returns>The results of executing the query.</returns>
        public virtual IList<TReturn> GetList()
        {
            return ExecuteGetList(Queryable);
        }

        /// <summary>
        /// Returns the count of items that match the query.
        /// </summary>
        /// <returns>Number of matches of executing the query.</returns>
        public int GetCount()
        {
            return ExecuteGetListSize(Queryable);
        }

        /// <summary>
        /// Constructs the query but doesn't execute it.
        /// </summary>
        /// <returns>The underlying query.</returns>
        public IQueryable<TReturn> GetUnderlyingQuery()
        {
            return CreateGetListQuery(Queryable);
        }

        /// <summary>
        /// Executes the query to find out whether any items exist or not.
        /// </summary>
        /// <returns><c>true</c> if there are items that match the query; otherwise, <c>false</c>.</returns>
        public virtual bool GetExistsAsync()
        {
            return ExecuteGetListSize(Queryable) > 0;
        }

        /// <summary>
        /// Executes the query to find out whether any items exist or not.
        /// </summary>
        /// <returns><c>true</c> if there are items that match the query; otherwise, <c>false</c>.</returns>
        public bool GetExists()
        {
            return ExecuteGetListSize(Queryable) > 0;
        }

        /// <summary>
        /// Executes the specified Queryable.
        /// </summary>
        /// <param name="queryable">The queryable to query against.</param>
        /// <returns></returns>
        /// <remarks>Intended for use only by test code.</remarks>
        public virtual ResultList<TReturn> GetResultList(IQueryable<TQuery> queryable)
        {
            return ExecuteResultList(queryable);
        }

        /// <summary>
        /// Executes the single if exists.
        /// </summary>
        /// <returns></returns>
        public virtual TReturn GetSingleIfExists()
        {
            return GetList().FirstOrDefault();
        }

        /// <summary>
        /// Executes the single.
        /// </summary>
        /// <returns></returns>
        public virtual TReturn GetSingle()
        {
            var bo = GetSingleIfExists();

            if (ReferenceEquals(bo, null))
            {
                throw new MissingEntityException(typeof(TReturn));
            }

            return bo;
        }

        /// <summary>
        /// Gets the size of the collection.
        /// </summary>
        /// <param name="queryable">The queryable to query against.</param>
        /// <returns></returns>
        public virtual int GetResultListSize(IQueryable<TQuery> queryable)
        {
            return ExecuteGetListSize(queryable);
        }

        /// <summary>
        /// Returns a flag indicating if the query returned any items.
        /// </summary>
        /// <returns><c>true</c> if the query returned any items; otherwise, <c>false</c>.</returns>
        public virtual bool GetAnyItemsExist()
        {
            return GetResultListSize(Queryable) > 0;
        }

        protected virtual IQueryable<TReturn> CreateGetListQuery(IQueryable<TQuery> queryable)
        {
            var query = FormLinqQuery(queryable);

            // Handling of Projections differs
            if (IsProjection)
            {
                var ret = AddProjection(query);

                if (ShouldBeTruncated)
                {
                    ret = ret.Skip(PageNumber * PageSize).Take(PageSize);
                }

                return ret;
            }

            query = ShouldBeTruncated ? query.Skip(PageNumber * PageSize).Take(PageSize) : query;
            return query.Cast<TReturn>();
        }

        /// <summary>
        /// Executes the get collection.
        /// </summary>
        /// <returns></returns>
        protected virtual List<TReturn> ExecuteGetList(IQueryable<TQuery> queryable)
        {
            return CreateGetListQuery(queryable).ToList();
        }

        /// <summary>
        /// Executes the size of the get collection.
        /// </summary>
        /// <param name="queryable">The queryable to query against.</param>
        /// <returns></returns>
        protected virtual int ExecuteGetListSize(IQueryable<TQuery> queryable)
        {
            return FormLinqQuery(queryable).Count();
        }

        /// <summary>
        /// Executes the sum of the get collection.
        /// </summary>
        /// <param name="queryable">The queryable to query against.</param>
        /// <returns>The total sum of the numbers returned by the query.</returns>
        protected virtual decimal ExecuteGetListSum(IQueryable<TQuery> queryable) { throw new NotImplementedException("override to retrieve a sum"); }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="queryable">The queryable to query against.</param>
        /// <returns></returns>
        protected virtual ResultList<TReturn> ExecuteResultList(IQueryable<TQuery> queryable)
        {
            if (ShouldBeTruncated)
            {
                var totalNumberOfRecords = ExecuteGetListSize(queryable);
                var totalNumberOfPages = totalNumberOfRecords == 0 ? 0 : (totalNumberOfRecords - 1) / PageSize;

                // Sanity check here, if the call has just been made with a new page size, but the page is now out of range, then we need to change the page size
                if (PageNumber > 0 && PageNumber > totalNumberOfPages)
                {
                    PageNumber = totalNumberOfPages;
                }

                var executeGetList = ExecuteGetList(queryable);
                
                return (new ResultList<TReturn>(executeGetList, PageNumber < totalNumberOfPages, totalNumberOfRecords, totalNumberOfPages + 1, PageSize, PageNumber));
            }

            return NewResultList(ExecuteGetList(queryable));
        }

        #endregion


        #region Truncation

        /// <summary>
        /// Creates a new Result list based on whether the query results should be truncated.
        /// </summary>
        /// <param name="list">The list returned from the query execution.</param>
        /// <returns>The new list.</returns>
        protected ResultList<TReturn> NewResultList(IList<TReturn> list)
        {
            ResultList<TReturn> boCollection;

            if (ShouldBeTruncated)
            {
                if (list.Count == PageSize + 1)
                {
                    list.RemoveAt(list.Count - 1);
                    boCollection = new ResultList<TReturn>(list, true);
                }
                else
                {
                    boCollection = new ResultList<TReturn>(list, false);
                }
            }
            else
            {
                boCollection = new ResultList<TReturn>(list);
            }

            return boCollection;
        }

        #endregion


        #region Projections

        /// <summary>
        /// Gets a value indicating whether this instance is a projection query.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is projection; otherwise, <c>false</c>.
        /// </value>
        protected static bool IsProjection => typeof(IProjection).IsAssignableFrom(typeof(TReturn));

        /// <summary>
        /// Adds the projection to the query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">Thrown when the target types projection implementation can't be found.</exception>
        protected static IQueryable<TReturn> AddProjection(IQueryable<TQuery> query)
        {
            try
            {
                return (IQueryable<TReturn>)typeof(TReturn).InvokeMember("AddProjection", BindingFlags.InvokeMethod, null, null, new[] { query });
            }
            catch (Exception exception)
            {
                throw new NotImplementedException(string.Format("You need to add the implementation of 'static IQueryable<{0}> AddProjection(IQueryable<{1}> query)' to the type {0}", typeof(TReturn), typeof(TQuery)), exception);
            }
        }

        #endregion


        #region To be overridden

        /// <summary>
        /// Forms the linq query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        protected abstract IQueryable<TQuery> FormLinqQuery(IQueryable<TQuery> query);

        #endregion
    }
}