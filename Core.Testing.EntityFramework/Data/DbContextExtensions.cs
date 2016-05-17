using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;
using Sfa.Core.Context;
using Sfa.Core.Logging;
using Sfa.Core.Testing;

namespace Sfa.Core.Data
{
    /// <summary>
    /// Extension helpers for <see cref="DbContext"/>.
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Deletes all rows for the given table name.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> instance to use.</param>
        /// <param name="tableName">the table to delete the rows from.</param>
        public static void DeleteTable(this DbContext context, string tableName)
        {
            ApplicationContext.Logger.Log(LoggingLevel.Debug, typeof (BaseEntityFrameworkDatabaseTest).FullName, () => "About to delete rows from {0}", tableName);
            
            const string sql = @"SET ROWCOUNT 10000
                                
                                WHILE 1=1
                                BEGIN
                                   DELETE FROM {0};
   
                                   IF @@ROWCOUNT = 0
	                                   BREAK;
                                END;";

            int rowCount;
            do
            {
                rowCount = ExecuteSqlCommand(context, string.Format(sql, tableName));

                ApplicationContext.Logger.Log(LoggingLevel.Debug, typeof(BaseEntityFrameworkDatabaseTest).FullName, () => "Deleted {0} rows from {1}", rowCount, tableName);
            }
            while (rowCount > 0);
            context.Database.ExecuteSqlCommand("SET ROWCOUNT 0");
        }


        /// <summary>
        /// Deletes all rows for the given table name.
        /// </summary>
        /// <typeparam name="T">The entity type whose table should be deleted.</typeparam>
        /// <param name="context">The <see cref="DbContext"/> instance to use.</param>
        public static void DeleteTable<T>(this DbContext context) where T : class
        {
            var tableName = context.GetTableName<T>();
            context.DeleteTable(tableName);
        }


        /// <summary>
        /// Deletes all rows for the given table name using a truncate statement.
        /// </summary>
        /// <typeparam name="T">The entity type whose table should be deleted.</typeparam>
        /// <param name="context">The <see cref="DbContext"/> instance to use.</param>
        public static void TruncateTable<T>(this DbContext context) where T : class
        {
            var tableName = context.GetTableName<T>();
            context.TruncateTable(tableName);
        }


        /// <summary>
        /// Deletes all rows for the given table name using a truncate statement.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> instance to use.</param>
        /// <param name="tableName">the table to delete the rows from.</param>
        public static void TruncateTable(this DbContext context, string tableName)
        {
            ApplicationContext.Logger.Log(LoggingLevel.Debug, typeof(BaseEntityFrameworkDatabaseTest).FullName, () => "About to truncate {0}", tableName);

            const string sql = "TRUNCATE TABLE {0}";

            ExecuteSqlCommand(context, string.Format(sql, tableName));

            ApplicationContext.Logger.Log(LoggingLevel.Debug, typeof(BaseEntityFrameworkDatabaseTest).FullName, () => "Truncated {0}", tableName);
        }


        /// <summary>
        /// Executes the raw sql against the given context.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> instance to use.</param>
        /// <param name="sql">The sql to execute.</param>
        /// <returns>The number of rows affected by the command.</returns>
        private static int ExecuteSqlCommand(this DbContext context, string sql)
        {
            var commandTimeout = context.Database.CommandTimeout;
            context.Database.CommandTimeout = Int32.MaxValue;
            var rowCount = context.Database.ExecuteSqlCommand(sql);
            context.Database.CommandTimeout = commandTimeout;

            return rowCount;
        }


        /// <summary>
        /// Deletes the entity from the data store for the supplied type.
        /// </summary>
        /// <typeparam name="T">The type of the entity to remove all instances for.</typeparam>
        /// <param name="context">The <see cref="DbContext"/> instance to use.</param>
        /// <param name="set">The items to delete.</param>
        public static void Delete<T>(this DbContext context, DbSet<T> set) where T : class
        {
            var tableName = context.GetTableName<T>();
            ApplicationContext.Logger.Log(LoggingLevel.Debug, typeof(BaseEntityFrameworkDatabaseTest).FullName, () => "About to delete rows from {0}", tableName);
            set.RemoveRange(set);
            context.SaveChanges();
        }


        /// <summary>
        /// Gets the table name for the entity type parameter supplied.
        /// </summary>
        /// <typeparam name="T">The type of the entity to get the underlying table name for.</typeparam>
        /// <param name="context">The <see cref="DbContext"/> instance to use.</param>
        /// <returns>The name of the table for the supplied entity type.</returns>
        public static string GetTableName<T>(this DbContext context) where T : class
        {
            var objectContext = ((IObjectContextAdapter)context).ObjectContext;

            return objectContext.GetTableName<T>();
        }


        /// <summary>
        /// Gets the table name for the entity type parameter supplied.
        /// </summary>
        /// <typeparam name="T">The type of the entity to get the underlying table name for.</typeparam>
        /// <param name="context">The <see cref="ObjectContext"/> instance to use.</param>
        /// <returns>The name of the table for the supplied entity type.</returns>
        public static string GetTableName<T>(this ObjectContext context) where T : class
        {
            var sql = context.CreateObjectSet<T>().ToTraceString();
            var regex = new Regex("FROM (?<table>.*) AS");
            var match = regex.Match(sql);

            var table = match.Groups["table"].Value;
            return table;
        }
    }
}