using Sfa.Core.Data;
using Sfa.Core.Entities;
using Sfa.MyProject.Contexts;
using Sfa.MyProject.Domain.EntityFramework.ProjectTemplate.Domain;

namespace Sfa.MyProject.Domain.EntityFramework.ProjectTemplate.Contexts
{
    public class EntityFrameworkQueryFactory : IQueryFactory
    {
        #region Common

        /// <summary>
        /// Creates a new query for searching for an item by its Id.
        /// </summary>
        /// <typeparam name="TReturn">The type to return from the execution of the query.</typeparam>
        /// <typeparam name="TQuery">The type to search for.</typeparam>
        /// <param name="id">The id of the instance.</param>
        /// <returns>The new query.</returns>
        public IQuery<TReturn> NewAllForIdQuery<TReturn, TQuery>(int id)
            where TQuery : class, IEntity<int>
        {
            return new CommonQueries.ForId<TReturn, TQuery>(id);
        }

        #endregion
    }
}