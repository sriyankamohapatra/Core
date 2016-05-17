using System;
using Sfa.Core.Data;

namespace Sfa.Core.Context
{
    /// <summary>
    /// Context for tests.
    /// </summary>
    public static class AsyncTestContext
    {
        #region Fields
        
        private static readonly IContextStorage ContextStorage = new CallContextStorage();
        private static readonly string StorageKeyRepository = $"{typeof(TestContext).FullName}-asyncrepo";
        private static Func<IAsyncRepository> _createNewRepo;

        public static event EventHandler OnReset;

        #endregion


        #region Core Properties
        
        /// <summary>
        /// The logged for the system.
        /// </summary>
        public static IAsyncRepository Repository => ContextStorage?.GetData<IAsyncRepository>(StorageKeyRepository);

        /// <summary>
        /// Creates a new Repository.
        /// </summary>
        public static IAsyncRepository NewRepository => _createNewRepo();

        #endregion


        #region Life cycle

        /// <summary>
        /// Sets up the context with the appropriate dependencies.
        /// </summary>
        /// <param name="repository">The repository for the tests.</param>
        /// <param name="createNewRepo">Function that will create a new repository.</param>
        public static void Setup(IAsyncRepository repository, Func<IAsyncRepository> createNewRepo)
        {
            ContextStorage.SetData(StorageKeyRepository, repository);
            _createNewRepo = createNewRepo;
        }

        /// <summary>
        /// Tears down all the dependencies of the system.
        /// </summary>
        public static void TearDown()
        {
            ContextStorage.CleanAndDispose(StorageKeyRepository);
        }

        public static void Reset()
        {
            OnReset?.Invoke(null, EventArgs.Empty);
        }

        #endregion
    }
}