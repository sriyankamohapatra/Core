using System.Data.Entity.SqlServer;
using Sfa.Core.Data;
using Sfa.Core.Entities;

namespace Sfa.Core.Reflection
{
    public abstract class EntityFrameworkEntityBuilder<T, TId> : EntityBuilder<T, TId>
        where T : class, IEntity<TId>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkEntityBuilder{T, TId}"/> class.
        /// </summary>
        /// <param name="defaultBo">The default entity.</param>
        protected EntityFrameworkEntityBuilder(T defaultBo)
            : base(defaultBo)
        {

        }

        #endregion

        protected override void OnPersistForAlreadySavedEntity<TEntity>(IRepository repository, TEntity entity)
        {
            base.OnPersistForAlreadySavedEntity(repository, entity);
            var efRepo = (EntityFrameworkRepository)repository;

            efRepo.Context.Set<TEntity>().Attach(entity);
        }

        protected override void OnPersistForAlreadySavedEntity<TEntity, TEntityId>(IRepository repository, TEntity entity)
        {
            base.OnPersistForAlreadySavedEntity<TEntity, TEntityId>(repository, entity);
            var efRepo = (EntityFrameworkRepository)repository;

            efRepo.Context.Set<TEntity>().Attach(entity);
        }

        public override T BuildAndPersist(IRepository repository = null)
        {
            if (repository == null)
            {
                var executionStrategy = new SqlAzureExecutionStrategy();

                SqlAzureDbConfiguration.SuspendExecutionStrategy = true;

                var entity = executionStrategy.Execute(() => base.BuildAndPersist(null));

                SqlAzureDbConfiguration.SuspendExecutionStrategy = false;

                return entity;
            }

            return base.BuildAndPersist(repository);
        }
    }
}