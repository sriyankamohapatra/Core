using System;
using System.Data.Entity.SqlServer;
using Sfa.Core.Data;

namespace Sfa.$safeprojectname$.Testing
{
    public class BaseCommandTest : BaseQueryTest
    {
        #region Life cycle

        protected override void SetUpEachTest()
        {
            base.SetUpEachTest();
        }


        protected override void TearDownEachTest()
        {
            base.TearDownEachTest();
        }

        #endregion


        #region Execution Helpers

        protected void RunInExecutionWrapper(Action run)
        {
            // We wrap this so it will repeat the whole test if there is a timeout or
            // we lose connectivity in the cloud.
            var executionStrategy = new SqlAzureExecutionStrategy();

            SqlAzureDbConfiguration.SuspendExecutionStrategy = true;

            executionStrategy.Execute(run);
        }

        protected T RunInExecutionWrapper<T>(Func<T> run)
        {
            // We wrap this so it will repeat the whole test if there is a timeout or
            // we lose connectivity in the cloud.
            var executionStrategy = new SqlAzureExecutionStrategy();

            SqlAzureDbConfiguration.SuspendExecutionStrategy = true;

            return executionStrategy.Execute(run);
        }

        #endregion
    }
}