using Sfa.Core.Data;
using Sfa.$safeprojectname$.Contexts;

namespace Sfa.$safeprojectname$.Testing
{
    public class BaseQueryTest : BaseDataIntegrationTest
    {
        #region Life cycle

        protected override void SetUpEachTest()
        {
            base.SetUpEachTest();

            SetupDomainContexts();
        }


        protected override void TearDownEachTest()
        {
            TearDownDomainContexts();

            base.TearDownEachTest();
        }

        protected virtual void SetupDomainContexts()
        {

            DomainContext.Setup(new EntityFrameworkRepository(BootstrapContext()),
                new DefaultDomainFactory(),
                new EntityFrameworkQueryFactory());
        }

        protected virtual void TearDownDomainContexts()
        {
            DomainContext.TearDown();
        }

        #endregion
    }
}