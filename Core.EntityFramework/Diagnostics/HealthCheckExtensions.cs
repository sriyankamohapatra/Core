using System;
using System.Data.Entity;

namespace Sfa.Core.Diagnostics
{
    /// <summary>
    /// Entity framework extensions for <c>HealthCheck</c>.
    /// </summary>
    public static class HealthCheckExtensions
    {
        public static void RunDbContextTest<T>(this HealthCheck healthCheck, string name)
            where T : DbContext, new()
        {
            healthCheck.RunTest(() =>
            {
                using (var db = new T())
                {
                    using (var conn = db.Database.Connection)
                    {
                        conn.Open();
                    }
                }
            }, name);
        }


        public static void RunDbContextTest<T>(this HealthCheck healthCheck, string connectionStringOrName, string name)
            where T : DbContext
        {
            healthCheck.RunTest(() =>
            {
                using (var db = (T)Activator.CreateInstance(typeof(T), connectionStringOrName))
                {
                    using (var conn = db.Database.Connection)
                    {
                        conn.Open();
                    }
                }
            }, name);
        }
    }
}