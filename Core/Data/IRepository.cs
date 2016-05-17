using System;
using System.Linq;
using Sfa.Core.Entities;

namespace Sfa.Core.Data
{
    /// <summary>
    /// Represents the Repository for storing and accessing data for a domain.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Creates the entity in the Repository.
        /// </summary>
        /// <typeparam name="T">The type of Entity that is being created.</typeparam>
        /// <param name="toSave">The instance to create.</param>
        /// <returns>The actual instance created.</returns>
        T Create<T>(T toSave)
            where T : class, IEntity;

        /// <summary>
        /// Updates the entity within the Repository.
        /// </summary>
        /// <typeparam name="T">The type of entity being updated.</typeparam>
        /// <param name="entity">The instance to update.</param>
        /// <returns>The actual instance updated.</returns>
        T Update<T>(T entity)
            where T : class, IEntity;

        /// <summary>
        /// Deletes an entity from the Repository.
        /// </summary>
        /// <typeparam name="T">The type of entity to delete.</typeparam>
        /// <param name="entity">The instance to delete.</param>
        /// <returns>The actual instance deleted.</returns>
        T Delete<T>(T entity)
            where T : class, IEntity;

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
        /// Saves all outstanding changes to the repository.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Gets a queryable instance for the type specified.
        /// </summary>
        /// <typeparam name="T">The type to get the queryable instance for.</typeparam>
        /// <returns>The queryable for the specified type.</returns>
        IQueryable<T> GetQueryable<T>() where T : class;

        /// <summary>
        /// Removes all entries from the repository.
        /// </summary>
        /// <returns>The continuation task.</returns>
        void Clear();
    }
}