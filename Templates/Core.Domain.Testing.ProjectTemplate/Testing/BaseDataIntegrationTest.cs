using System.Collections.Generic;
using System.Reflection;
using Sfa.Core.Context;
using Sfa.Core.Data;
using Sfa.Core.Equality;
using Sfa.Core.Net.Mail;
using Sfa.Core.Testing;
using Sfa.MyProject.Data;
using Sfa.MyProject.Domain;
using Sfa.MyProject.Domain.Builders;

namespace Sfa.MyProject.Testing
{
    /// <summary>
    /// Base class for all test that integrate with the azure emulator.
    /// </summary>
    public class BaseDataIntegrationTest : BaseEntityFrameworkDatabaseTest
    {
        #region Life Cycle

        /// <summary>
        /// Ensures that the correct classes will use value equality during testing.
        /// </summary>
        protected override IEnumerable<Assembly> AssembliesWithTypesToPerformFieldValueEqualityOn
        {
            get
            {
                yield return typeof(Root).Assembly;
            }
        }

        protected override void SetUpEachTest()
        {
            base.SetUpEachTest();
            FieldValueEqualityComparer.UseDateTimeEqualityComparer(new SqlServerDateTimeEqualityComparer());
            

            // Reset so that fresh for each test.
            SqlAzureDbConfiguration.SuspendExecutionStrategy = false;

            BootstrapContext(true, true);

            Core.Context.TestContext.Setup(new EntityFrameworkRepository(BootstrapContext()),
                () => new EntityFrameworkRepository(BootstrapContext()));
        }

        protected override void TearDownEachTest()
        {
            Core.Context.TestContext.TearDown();

            SimpleSmtpServer.EnsureAllEmailsPickedUp();

            base.TearDownEachTest();
        }

        #endregion


        #region DbContext

        protected virtual DomainDbContext BootstrapContext(bool deleteData = false, bool initialize = false)
        {
            var dbContext = new DomainDbContext();

            if (deleteData)
            {
                TeardownData(dbContext);
                dbContext.SaveChanges();
            }
            if (initialize)
            {
                dbContext.Database.Initialize(true);
            }

            return dbContext;
        }

        protected virtual void TeardownData(DomainDbContext context)
        {
            //context.DeleteTable<Audit>();
        }

        #endregion


        #region Builders

        protected RootBuilder NewRoot => new RootBuilder();

        #endregion
    }
}