using System;
using System.Data.Entity;
using System.Linq;
using Sfa.Core.Entities;

namespace Sfa.Core.Data
{
    public abstract class BaseEntityFrameworkRepository : ITransactional, IMaxEntityId, IDisposable
    {
        #region Fields

        private DbContextTransaction _dbContextTransaction;

        #endregion


        #region Core Properties

        /// <summary>
        /// The underlying context.
        /// </summary>
        public DbContext Context { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="dbContext">The underlying data base context.</param>
        protected BaseEntityFrameworkRepository(DbContext dbContext)
        {
            Context = dbContext;
        }

        #endregion


        #region Repository Implementation
        
        /// <summary>
        /// Gets a queryable instance for the type specified.
        /// </summary>
        /// <typeparam name="T">The type to get the queryable instance for.</typeparam>
        /// <returns>The queryable for the specified type.</returns>
        public IQueryable<T> GetQueryable<T>()
            where T : class
        {
            return Context.Set<T>();
        }

        #endregion


        #region ITransactional Implementation

        /// <summary>
        /// Flag indicating if there exists a current transaction for the actions.
        /// </summary>
        public bool HasCurrenTransaction => _dbContextTransaction != null;

        /// <summary>
        /// Creates a transaction for all pending actions on the Repository.
        /// </summary>
        public void BeginTransaction()
        {
            _dbContextTransaction = Context.Database.BeginTransaction();
        }

        /// <summary>
        /// Commits the transaction that contains all pending actions on the Repository.
        /// </summary>
        public void CommitTransaction()
        {
            _dbContextTransaction.Commit();
            _dbContextTransaction.Dispose();
            _dbContextTransaction = null;
        }

        /// <summary>
        /// Rolls back the transaction that contains all pending actions on the Repository.
        /// </summary>
        public void RollbackTransaction()
        {
            _dbContextTransaction?.Rollback();
            _dbContextTransaction?.Dispose();
            _dbContextTransaction = null;
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Returns the Max Id for the Entity type specified.
        /// </summary>
        /// <typeparam name="T">The type of the Entity to get the Max id for.</typeparam>
        /// <typeparam name="TId">The type of the id.</typeparam>
        /// <returns>The maximum id for the entity type.</returns>
        public TId GetMaxId<T, TId>() where T : class, IEntity<TId>
        {
            return Context.Set<T>().Max(e => e.Id);
        }

        #endregion


        #region IDisposable Implementation

        /// <summary>
        /// Provides a mechanism for releasing unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //_dbContextTransaction?.Dispose();
                Context?.Dispose();
            }
        }

        #endregion
    }
}