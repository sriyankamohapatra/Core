using Sfa.Core.Context;
using Sfa.Core.Data;

namespace Sfa.MyProject.Contexts
{
    /// <summary>
    /// Defines the contexts appropriate for the contracts domain.
    /// </summary>
    public class DomainContext
    {
        #region Fields

        private static IContextStorage _contextStorage;
        private static readonly string StorageKeyRepository = $"{typeof(DomainContext).FullName}-Repository";
        private static readonly string StorageKeyDomainFactory = $"{typeof(DomainContext).FullName}-DomainFactory";
        private static readonly string StorageKeyQueryFactory = $"{typeof(DomainContext).FullName}-QueryFactory";

        #endregion


        #region Core Properties

        /// <summary>
        /// The document content repository for the domain.
        /// </summary>
        public static IRepository Repository => _contextStorage.GetData<IRepository>(StorageKeyRepository);

        /// <summary>
        /// The instance factory for the domain.
        /// </summary>
        public static IDomainFactory DomainFactory => _contextStorage.GetData<IDomainFactory>(StorageKeyDomainFactory);

        /// <summary>
        /// The query factory for the domain.
        /// </summary>
        public static IQueryFactory QueryFactory => _contextStorage.GetData<IQueryFactory>(StorageKeyQueryFactory);


        #endregion


        #region Life cycle

        /// <summary>
        /// Sets the context storage to use for this type of context.
        /// </summary>
        /// <param name="contextStorage">The context storage implementation.</param>
        public static void SetContextStorage(IContextStorage contextStorage)
        {
            _contextStorage = contextStorage;
        }

        /// <summary>
        /// Sets up the context with the appropriate dependencies.
        /// </summary>
        /// <param name="repository">The repository for the domain.</param>
        /// <param name="domainFactory">The domain instance factory for the domain.</param>
        /// <param name="queryFactory">The query factory for the domain.</param>
        public static void Setup(IRepository repository,
                                 IDomainFactory domainFactory,
                                 IQueryFactory queryFactory)
        {
            _contextStorage.SetData(StorageKeyRepository, repository);
            _contextStorage.SetData(StorageKeyDomainFactory, domainFactory);
            _contextStorage.SetData(StorageKeyQueryFactory, queryFactory);
        }

        /// <summary>
        /// Tears down all the dependencies for this instance.
        /// </summary>
        public static void TearDown()
        {
            _contextStorage.CleanAndDispose(StorageKeyRepository, StorageKeyQueryFactory, StorageKeyDomainFactory);
        }

        #endregion 
    }
}