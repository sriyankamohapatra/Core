using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Runtime.Remoting.Messaging;

namespace Sfa.Core.Data
{
    public class SqlAzureDbConfiguration : DbConfiguration
    {
        public SqlAzureDbConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => SuspendExecutionStrategy
                 ? (IDbExecutionStrategy)new DefaultExecutionStrategy()
                 : new SqlAzureExecutionStrategy());
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