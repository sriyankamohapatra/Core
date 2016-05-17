using System.Data.Entity.SqlServer;
using System.Threading;
using System.Threading.Tasks;
using Sfa.Core.Data;
using Sfa.Core.Entities;

namespace Sfa.Core.Reflection
{
    public abstract class AsyncEntityFrameworkEntityBuilder<T, TId> : AsyncEntityBuilder<T, TId>
        where T : class, IEntity<TId>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncEntityFrameworkEntityBuilder{T, TId}"/> class.
        /// </summary>
        /// <param name="defaultBo">The default entity.</param>
        protected AsyncEntityFrameworkEntityBuilder(T defaultBo)
            : base(defaultBo)
        {

        }

        #endregion

        protected override async Task OnPersistForAlreadySavedEntityAsync<TEntity>(IAsyncRepository repository, TEntity entity)
        {
            await base.OnPersistForAlreadySavedEntityAsync(repository, entity);
            var efRepo = (EntityFrameworkAsyncRepository)repository;
            efRepo.Context.Set<TEntity>().Attach(entity);
        }

        public override async Task<T> BuildAndPersistAsync(IAsyncRepository repository = null)
        {
            if (repository == null)
            {
                var executionStrategy = new SqlAzureExecutionStrategy();

                SqlAzureDbConfiguration.SuspendExecutionStrategy = true;

                var entity = await executionStrategy.ExecuteAsync(() => base.BuildAndPersistAsync(null), CancellationToken.None);

                SqlAzureDbConfiguration.SuspendExecutionStrategy = false;

                return entity;
            }

            return await base.BuildAndPersistAsync(repository);
        }
    }
}