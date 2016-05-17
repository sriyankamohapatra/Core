using System;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace LockheedMartin.Core.Data
{
    /// <summary>
    /// Linq to Sql implementation of the repository pattern.
    /// </summary>
    public class LinqToSqlRepository : IRepository
    {
        #region Fields

        private readonly DataContext _dataContext;

        #endregion


        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="dataContext">The underlying <see cref="DataContext"/></param>
        public LinqToSqlRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
            _dataContext.Connection.Open();
        }

        #endregion


        #region IRepository Impl

        /// <summary>
        /// Creates the entity in the Repository.
        /// </summary>
        /// <typeparam name="T">The type of Entity that is being created.</typeparam>
        /// <param name="toSave">The instance to create.</param>
        /// <returns>The actual instance to create.</returns>
        public T Create<T>(T toSave) where T : class
        {
            _dataContext.GetTable<T>().InsertOnSubmit(toSave);
            return toSave;
        }

        /// <summary>
        /// Updates the entity within the Repository.
        /// </summary>
        /// <typeparam name="T">The type of entity being updated.</typeparam>
        /// <param name="entity">The instance to update.</param>
        public void Update<T>(T entity) where T : class, IEntity
        {
            // Don't do anything here!
        }

        /// <summary>
        /// Deletes an entity from the Respository.
        /// </summary>
        /// <typeparam name="T">The type of entity to delete.</typeparam>
        /// <param name="entity">The instance to delete.</param>
        public void Delete<T>(T entity) where T : class, IEntity
        {
            _dataContext.GetTable<T>().DeleteOnSubmit(entity);
        }

        /// <summary>
        /// Loads an instance from the repository and optionally loads any relevant entities to the main entity.
        /// </summary>
        /// <typeparam name="T">The type of entity to load.</typeparam>
        /// <typeparam name="TId">The type of the id of the entity being loaded.</typeparam>
        /// <param name="id">The id of the entity being loaded.</param>
        /// <param name="includes">Any optional other entities releated to the main entity to load.</param>
        /// <returns>The entity with the specified id.</returns>
        public T Load<T, TId>(TId id, params Expression<Func<T, object>>[] includes) where T : class, IEntity<TId> where TId : IEquatable<TId>
        {
            //if (includes.Length == 0)
            {
                return _dataContext.GetTable<T>().FirstOrDefault(e => e.Id.Equals(id));
            }

            //IQueryable<T> query = _dbContext.Set<T>();

            //query = includes.Aggregate(query, (current, include) => current.Include(include));

            //return query.FirstOrDefaultAsync(o => o.Id.Equals(id)).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads the element directly using sql. This could be a stored procedure or straight sql.
        /// </summary>
        /// <typeparam name="T">The type of the element that is mapped from the sql.</typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns>The instance that matches the query, or <c>null</c>.</returns>
        public T LoadViaSqlQuery<T>(string sql, params object[] parameters)
        {
            return _dataContext.ExecuteQuery<T>(sql, parameters).FirstOrDefault();
        }

        /// <summary>
        /// Saves all outstanding changes to the repository.
        /// </summary>
        public void SaveChanges()
        {
            _dataContext.SubmitChanges();
        }

        /// <summary>
        /// Flag indicating if there exists a current transaction for the actions.
        /// </summary>
        public bool HasCurrenTransaction 
        {
            get
            {
                return _dataContext.Transaction != null;
            }
        }

        /// <summary>
        /// Creates a transaction for all pending actions on the Repository.
        /// </summary>
        public void BeginTransaction()
        {
            _dataContext.Transaction = _dataContext.Connection.BeginTransaction();
        }

        /// <summary>
        /// Commits the transaction that contains all pending actions on the Repository.
        /// </summary>
        public void CommitTransaction()
        {
            _dataContext.Transaction.Commit();
        }

        /// <summary>
        /// Rolls back the transaction that contains all pending actions on the Repository.
        /// </summary>
        public void RollbackTransaction()
        {
            _dataContext.Transaction.Rollback();
        }

        /// <summary>
        /// Returns the Max Id for the Entity type specified.
        /// </summary>
        /// <typeparam name="T">The type of the Entity to get the Max id for.</typeparam>
        /// <typeparam name="TId">The type of the id.</typeparam>
        /// <returns>The max id for the entity type.</returns>
        public TId GetMaxId<T, TId>() where T : class, IEntity<TId>
        {
            return _dataContext.GetTable<T>().Max(e => e.Id);
        }

        /// <summary>
        /// Gets a queryable instance for the type specified.
        /// </summary>
        /// <typeparam name="T">The type to get the queryable instance for.</typeparam>
        /// <returns>The queryable for the specified type.</returns>
        public IQueryable<T> GetQueryable<T>() 
            where T : class
        {
            return _dataContext.GetTable<T>();
        }
		 
	    #endregion
    }
}