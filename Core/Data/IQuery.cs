using System.Collections.Generic;

namespace Sfa.Core.Data
{
    /// <summary>
    /// Defines the interface of a Query<typeparam name="T"></typeparam>
    /// </summary>
    /// <typeparam name="T">The instance that is being queried against.</typeparam>
    public interface IQuery<T>
    {
        /// <summary>
        /// Returns a result list from the query.
        /// </summary>
        /// <returns>The result list from executing the query.</returns>
        IResultList<T> GetResultList(int pageNumber, int pageSize);

        /// <summary>
        /// Gets a list of all results from executing the query.
        /// </summary>
        /// <returns>All the entities that match the query.</returns>
        IList<T> GetList();

        /// <summary>
        /// Gets the single entity from executing the query.
        /// </summary>
        /// <returns>The entity that matches the query.</returns>
        /// <exception cref="Sfa.Core.Exceptions.MissingEntityException">Thrown if the entity can't be found.</exception>
        T GetSingle();

        /// <summary>
        /// Gets the single entity from executing the query.
        /// </summary>
        /// <returns>The entity that matches the query.</returns>
        T GetSingleIfExists();

        /// <summary>
        /// Get the total count of all the entities that match the query.
        /// </summary>
        /// <returns>The total count of matched entities.</returns>
        int GetCount();

        /// <summary>
        /// A flag representing if the query matched any entities.
        /// </summary>
        /// <returns><c>true</c> if there exists any entities that match the query; otherwise, <c>false</c>.</returns>
        bool GetExists();
    }
}