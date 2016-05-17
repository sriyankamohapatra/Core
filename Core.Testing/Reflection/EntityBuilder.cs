using System;
using System.Diagnostics;
using Sfa.Core.Context;
using Sfa.Core.Data;
using Sfa.Core.Entities;
using Sfa.Core.Logging;

namespace Sfa.Core.Reflection
{
    /// <summary>
    /// Base class for building entities
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public abstract class EntityBuilder<T, TId> : BaseEntityBuilder<T, TId, EntityBuilder<T, TId>>
        where T : class, IEntity<TId>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBuilder{T, TId}"/> class.
        /// </summary>
        /// <param name="defaultBo">The default entity.</param>
        protected EntityBuilder(T defaultBo)
            : base(defaultBo)
        {

        }

        #endregion


        #region Build

        /// <summary>
        /// Builds the and persist.
        /// </summary>
        /// <returns></returns>
        public virtual T BuildAndPersist(IRepository repository = null)
        {
            if (repository == null)
            {
                repository = TestContext.NewRepository;
            }

            var transactionalRepo = repository as ITransactional;
            if (transactionalRepo != null)
            {
                var iOwnTransaction = !transactionalRepo.HasCurrenTransaction;
                if (iOwnTransaction)
                {
                    transactionalRepo.BeginTransaction();
                    try
                    {
                        PersistBoGraph(repository);
                        transactionalRepo.CommitTransaction();
                    }
                    catch (Exception)
                    {
                        transactionalRepo.RollbackTransaction();
                        throw;
                    }
                }
                else
                {
                    PersistBoGraph(repository);
                }
            }
            else
            {
                PersistBoGraph(repository);
            }

            return Target;
        }

        private void PersistBoGraph(IRepository repository)
        {
            ApplicationContext.Logger.Log(LoggingLevel.Debug, "Builder", () => "Creating BO Graph for {0}", typeof(T));
            PersistAncestors(repository);
            PersistSelf(repository);
            PersistDescendants(repository);
        }

        /// <summary>
        /// Persist ancestors.
        /// </summary>
        /// <param name="repository"></param>
        protected abstract void PersistAncestors(IRepository repository);

        /// <summary>
        /// Persists the descendants.
        /// </summary>
        /// <param name="repository"></param>
        protected abstract void PersistDescendants(IRepository repository);

        /// <summary>
        /// Builds the and save.
        /// </summary>
        /// <returns></returns>
        protected T PersistSelf(IRepository repository)
        {
            var transactionalRepo = repository as ITransactional;
            if (transactionalRepo != null)
            {
                Debug.Assert(transactionalRepo.HasCurrenTransaction);
            }
            if (!AmIPersisted)
            {
                ApplicationContext.Logger.Log(LoggingLevel.Debug, "Builder", () => "Persisting {0}", typeof(T));
                OnBeforePersistSelf();
                repository.Create(Target);
                repository.SaveChanges();
                ApplicationContext.Logger.Log(LoggingLevel.Debug, "Builder", () => "Persisted {0} with Id: {1}", typeof(T), Target.Id);
            }
            else
            {
                ApplicationContext.Logger.Log(LoggingLevel.Debug, "Builder", () => "Skipping persist of {0} with id {1}", typeof(T), Target.Id);
            }

            return (Target);
        }

        /// <summary>
        /// Persists the specified entity.
        /// </summary>
        /// <param name="repository">The current repository for this build.</param>
        /// <param name="entity">The entity to persist.</param>
        protected void Persist<TBoBuilder, TEntity>(IRepository repository, TEntity entity)
            where TBoBuilder : EntityBuilder<TEntity, TId>, new()
            where TEntity : class, IEntity<TId>
        {
            // check first to see if the entity has already been persisted 
            // don't do anything if it has
            if (Equals(entity.Id, UnsavedId))
            {
                var builder = new TBoBuilder();
                builder.BuildAs(entity).BuildAndPersist(repository);
            }
            else
            {
                OnPersistForAlreadySavedEntity(repository, entity);
                ApplicationContext.Logger.Log(LoggingLevel.Debug, "Builder", () => "Skipping persist of {0} with id {1}", typeof(TEntity), entity.Id);
            }
        }

        protected virtual void OnPersistForAlreadySavedEntity<TEntity>(IRepository repository, TEntity entity)
            where TEntity : class, IEntity<TId>
        {
        }

        /// <summary>
        /// Persists the specified entity.
        /// </summary>
        /// <param name="repository">The current repository for this build.</param>
        /// <param name="entity">The entity to persist.</param>
        protected void Persist<TBoBuilder, TEntity, TEntityId>(IRepository repository, TEntity entity)
            where TBoBuilder : EntityBuilder<TEntity, TEntityId>, new()
            where TEntity : class, IEntity<TEntityId>
        {
            if (entity == null)
            {
                return;
            }

            // check first to see if the entity has already been persisted 
            // don't do anything if it has
            if (Equals(entity.Id, UnsavedId))
            {
                var builder = new TBoBuilder();
                builder.BuildAs(entity).BuildAndPersist(repository);
            }
            else
            {
                OnPersistForAlreadySavedEntity<TEntity, TEntityId>(repository, entity);
                ApplicationContext.Logger.Log(LoggingLevel.Debug, "Builder", () => "Skipping persist of {0} with id {1}", typeof(TEntity), entity.Id);
            }
        }

        protected virtual void OnPersistForAlreadySavedEntity<TEntity, TEntityId>(IRepository repository, TEntity entity)
            where TEntity : class, IEntity<TEntityId>, IEntity
        {
        }

        #endregion
    }
}