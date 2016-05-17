using Sfa.Core.Context;
using Sfa.Core.Data;
using Sfa.Core.Equality;

namespace Sfa.Core.Testing
{
    /// <summary>
    /// Base class for database tests when using Entity Framework.
    /// </summary>
    public class BaseEntityFrameworkDatabaseTest : BaseTest
    {
        #region Life Cycle

        protected override void SetUpEachTest()
        {
            base.SetUpEachTest();
            FieldValueEqualityComparer.AddFieldValueEqualityComparer(new ProxyFieldValueEqualityComparer());
            FieldValueEqualityComparer.AddFieldValueTypeEqualityComparer(new ProxyFieldValueTypeEqualityComparer());
        }

        #endregion


        #region Logging

        /// <summary>
        /// Enables the logging of Entity framework methods.
        /// </summary>
        public void EnableEntityFrameworkRepositoryLogging()
        {
            EnableLogging(typeof(EntityFrameworkAsyncRepository).FullName);
        }


        /// <summary>
        /// Enables logging of EF sql commands.
        /// </summary>
        public void EnableEntityFrameworkSqlLogging(IRepository repository)
        {
            var repo = repository as EntityFrameworkRepository;
            if (repo != null)
            {
                repo.Context.Database.Log = s => Log(s);
            }
            var asyncRepo = AsyncTestContext.Repository as EntityFrameworkAsyncRepository;
            if (asyncRepo != null)
            {
                asyncRepo.Context.Database.Log = s => Log(s);
            }
        }

        #endregion
    }
}