using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Runtime.Remoting.Messaging;
using Sfa.Core.Context;
using Sfa.Core.Logging;

namespace Sfa.Core.Data
{
    public class SqlAzureDbConfiguration : DbConfiguration
    {
        public SqlAzureDbConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => SuspendExecutionStrategy
                 ? GetDefaultExecutionStrategy()
                 : GetSqlAzureExecutionStrategy());
        }

        private IDbExecutionStrategy GetDefaultExecutionStrategy()
        {
            //ApplicationContext.Logger?.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => $"using the {typeof(DefaultExecutionStrategy)} for the executing strategy.");
            return new DefaultExecutionStrategy();
        }

        private IDbExecutionStrategy GetSqlAzureExecutionStrategy()
        {
            //ApplicationContext.Logger?.Log(LoggingLevel.Debug, CoreLoggingCategory.Diagnostics, () => $"using the {typeof(SqlAzureExecutionStrategy)} for the executing strategy.");
            return new SqlAzureExecutionStrategy();
        }

        public static bool SuspendExecutionStrategy
        {
            get
            {
                return (bool?)CallContext.LogicalGetData("SuspendExecutionStrategy") ?? false;
            }
            set
            {
                CallContext.LogicalSetData("SuspendExecutionStrategy", value);
            }
        }
    }
}