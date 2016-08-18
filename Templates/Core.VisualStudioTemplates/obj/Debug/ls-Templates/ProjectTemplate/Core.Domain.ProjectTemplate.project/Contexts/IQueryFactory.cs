using Sfa.Core.Data;
using Sfa.Core.Entities;

namespace Sfa.$safeprojectname$.Contexts
{
    /// <summary>
    /// Defines the implementation of the factory for creating queries of the domain objects.
    /// </summary>
    public interface IQueryFactory
    {
        #region Common

        /// <summary>
        /// Creates a new query for searching for an item by its Id.
        /// </summary>
        /// <typeparam name="TReturn">The type to return from the execution of the query.</typeparam>
        /// <typeparam name="TQuery">The type to search for.</typeparam>
        /// <param name="id">The id of the instance.</param>
        /// <returns>The new query.</returns>
        IQuery<TReturn> NewAllForIdQuery<TReturn, TQuery>(int id)
            where TQuery : class, IEntity<int>;

        #endregion
    }
}