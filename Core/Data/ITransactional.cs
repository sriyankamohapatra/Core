namespace Sfa.Core.Data
{
    /// <summary>
    /// Implementation of a class that supports transactional operations.
    /// </summary>
    public interface ITransactional
    {

        /// <summary>
        /// Flag indicating if there exists a current transaction for the actions.
        /// </summary>
        bool HasCurrenTransaction { get; }

        /// <summary>
        /// Creates a transaction for all pending actions on the Repository.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits the transaction that contains all pending actions on the Repository.
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Rolls back the transaction that contains all pending actions on the Repository.
        /// </summary>
        void RollbackTransaction();
    }
}