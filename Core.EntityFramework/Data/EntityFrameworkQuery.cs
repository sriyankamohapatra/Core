using System.Collections.Generic;
using System.Linq;

namespace Sfa.Core.Data
{
    /// <summary>
    /// Entity Framework implementation of the query pattern.
    /// </summary>
    /// <typeparam name="TReturn">The type of the classes returned by the execution of the query.</typeparam>
    /// <typeparam name="TQuery">The type of the target of the query.</typeparam>
    public abstract class EntityFrameworkQuery<TReturn, TQuery> : Query<TReturn, TQuery>
    {
        /// <summary>
        /// Executes the get collection.
        /// </summary>
        /// <returns></returns>
        protected override List<TReturn> ExecuteGetList(IQueryable<TQuery> queryable)
        {
            return CreateGetListQuery(queryable).ToList();
        }

        /// <summary>
        /// Executes the size of the get collection.
        /// </summary>
        /// <param name="queryable">The queryable to query against.</param>
        /// <returns></returns>
        protected override int ExecuteGetListSize(IQueryable<TQuery> queryable)
        {
            return FormLinqQuery(queryable).Count();
        }
    }
}