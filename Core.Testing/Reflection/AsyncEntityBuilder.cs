using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
    public abstract class AsyncEntityBuilder<T, TId> : BaseEntityBuilder<T, TId, AsyncEntityBuilder<T, TId>>
        where T : class, IEntity<TId>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncEntityBuilder{T,TId}"/> class.
        /// </summary>
        /// <param name="defaultBo">The default entity.</param>
        protected AsyncEntityBuilder(T defaultBo)
            : base(defaultBo)
        {

        }

        #endregion
        

        #region Build

        /// <summary>
        /// Builds the and persist.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<T> BuildAndPersistAsync(IAsyncRepository repository = null)
        {
            if (repository == null)
            {
                repository = AsyncTestContext.NewRepository;
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
                        await PersistBoGraphAsync(repository);
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
                    await PersistBoGraphAsync(repository);
                }
            }
            else
            {
                await PersistBoGraphAsync(repository);
            }

            return Target;
        }

        private async Task PersistBoGraphAsync(IAsyncRepository repository)
        {
            ApplicationContext.Logger.Log(LoggingLevel.Debug, "Builder", () => "Creating Bo Graph for {0}", typeof(T));
            await PersistAncestorsAsync(repository);
            await PersistSelfAsync(repository);
            await PersistDescendantsAsync(repository);
        }

        /// <summary>
        /// PersistAsync ancestors.
        /// </summary>
        protected abstract Task PersistAncestorsAsync(IAsyncRepository repository);

        /// <summary>
        /// Persists the descendants.
        /// </summary>
        protected abstract Task PersistDescendantsAsync(IAsyncRepository repository);

        /// <summary>
        /// Builds the and save.
        /// </summary>
        /// <returns></returns>
        protected async Task<T> PersistSelfAsync(IAsyncRepository repository)
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
                await repository.CreateAsync(Target);
                await repository.SaveChangesAsync();
                await repository.ClearAsync();
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
        protected async void PersistAsync<TBoBuilder, TEntity>(IAsyncRepository repository, TEntity entity)
            where TBoBuilder : AsyncEntityBuilder<TEntity, TId>, new()
            where TEntity : class, IEntity<TId>
        {
            // check first to see if the entity has already been persisted 
            // don't do anything if it has
            if (Equals(entity, UnsavedId))
            {
                var builder = new TBoBuilder();
                await builder.BuildAs(entity).BuildAndPersistAsync();
            }
            else
            {
                await OnPersistForAlreadySavedEntityAsync(repository, entity);
                ApplicationContext.Logger.Log(LoggingLevel.Debug, "Builder", () => "Skipping persist of {0} with id {1}", typeof(TEntity), entity.Id);
            }
        }

        protected virtual Task OnPersistForAlreadySavedEntityAsync<TEntity>(IAsyncRepository repository, TEntity entity)
            where TEntity : class, IEntity<TId>
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Persists the specified entity.
        /// </summary>
        /// <param name="entity">The entity to persist.</param>
        protected async Task PersistAsync<TBoBuilder, TEntity, TEntityId>(TEntity entity)
            where TBoBuilder : AsyncEntityBuilder<TEntity, TEntityId>, new()
            where TEntity : class, IEntity<TEntityId>
        {
            if (entity == null)
            {
                return;
            }

            var builder = new TBoBuilder();
            await builder.BuildAs(entity).BuildAndPersistAsync();
        }

        #endregion
    }
}