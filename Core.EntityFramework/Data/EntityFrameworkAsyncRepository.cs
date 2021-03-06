﻿using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using Sfa.Core.Context;
using Sfa.Core.Entities;
using Sfa.Core.Logging;

namespace Sfa.Core.Data
{
    /// <summary>
    /// Entity Framework implementation of a Repository for storing and accessing data for a domain.
    /// </summary>
    public class EntityFrameworkAsyncRepository : BaseEntityFrameworkRepository, IAsyncRepository
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="dbContext">The underlying data base context.</param>
        public EntityFrameworkAsyncRepository(DbContext dbContext)
            : base(dbContext)
        {
        }

        #endregion


        #region IAsyncRepository Implementation

        /// <summary>
        /// Creates the entity in the Repository.
        /// </summary>
        /// <typeparam name="T">The type of Entity that is being created.</typeparam>
        /// <param name="entity">The instance to create.</param>
        /// <returns>The actual instance to create.</returns>
        public Task<T> CreateAsync<T>(T entity) where T : class, IEntity
        {
            Context.Entry(entity).State = EntityState.Added;
            
            return Task.FromResult(entity);
        }

        /// <summary>
        /// Updates the entity within the Repository.
        /// </summary>
        /// <typeparam name="T">The type of entity being updated.</typeparam>
        /// <param name="entity">The instance to update.</param>
        /// <returns>The updated instance.</returns>
        public Task<T> UpdateAsync<T>(T entity) where T : class, IEntity
        {
            Context.Entry(entity).State = EntityState.Modified;
            
            return Task.FromResult(entity);
        }

        /// <summary>
        /// Deletes an entity from the Repository.
        /// </summary>
        /// <typeparam name="T">The type of entity to delete.</typeparam>
        /// <param name="entity">The instance to delete.</param>
        /// <returns>The deleted instance.</returns>
        public Task<T> DeleteAsync<T>(T entity) where T : class, IEntity
        {
            Context.Entry(entity).State = EntityState.Deleted;
            
            return Task.FromResult(entity);
        }

        /// <summary>
        /// Loads an instance from the repository and optionally loads any relevant entities to the main entity.
        /// </summary>
        /// <typeparam name="T">The type of entity to load.</typeparam>
        /// <typeparam name="TId">The type of the id of the entity being loaded.</typeparam>
        /// <param name="id">The id of the entity being loaded.</param>
        /// <returns>The entity with the specified id.</returns>
        public async Task<T> LoadAsync<T, TId>(TId id) where T : class, IEntity<TId> where TId : IEquatable<TId>
        {
            return await Context.Set<T>().FindAsync(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Saves all outstanding changes to the repository.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbEntityValidationException exception)
            {
                foreach (var error in exception.EntityValidationErrors.SelectMany(e => e.ValidationErrors))
                {
                    var error1 = error;
                    ApplicationContext.Logger.Log(LoggingLevel.Error, CoreLoggingCategory.Data, () => "{0} - {1}", error1.PropertyName, error1.ErrorMessage);
                }

                throw;
            }
        }

        /// <summary>
        /// Removes all entries from the repository.
        /// </summary>
        /// <returns>The continuation task.</returns>
        public Task ClearAsync()
        {
            ApplicationContext.Logger.Log(LoggingLevel.Debug, CoreLoggingCategory.Data, () => "Clearing all entries from EF context");
            var entries = Context.ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                Context.Entry(entry.Entity).State = EntityState.Detached;
            }
            return Task.FromResult(0);
        }

        #endregion
    }
}