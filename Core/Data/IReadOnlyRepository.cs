using System;
using System.Linq;
using Sfa.Core.Entities;

namespace Sfa.Core.Data
{
    /// <summary>
    /// Represents the Repository for only accessing data for a domain.
    /// </summary>
    public interface IReadOnlyRepository
    {
        /// <summary>
        /// Loads an instance from the repository..
        /// </summary>
        /// <typeparam name="T">The type of entity to load.</typeparam>
        /// <typeparam name="TId">The type of the id of the entity being loaded.</typeparam>
        /// <param name="id">The id of the entity being loaded.</param>
        /// <returns>The entity with the specified id.</returns>
        T Load<T, TId>(TId id)
            where T : class, IEntity<TId>
            where TId : IEquatable<TId>;

        /// <summary>
        /// Gets a queryable instance for the type specified.
        /// </summary>
        /// <typeparam name="T">The type to get the queryable instance for.</typeparam>
        /// <returns>The queryable for the specified type.</returns>
        IQueryable<T> GetQueryable<T>() where T : class;
    }
}