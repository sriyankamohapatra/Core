using System.Data.Entity;
using Sfa.Core.Data;

namespace $safeprojectname$.Data
{
    /// <summary>
    /// Represents DB access wrapper for the Entity Framework.
    /// </summary>
    [DbConfigurationType(typeof(SqlAzureDbConfiguration))]
    public class DomainDbContext : DbContext
    {
        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public DomainDbContext()
            : this("myproject") // TODO change this
        {
        }

        /// <summary>
        /// Connection string constructor.
        /// </summary>
        public DomainDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<DomainDbContext, Configuration>(nameOrConnectionString));
        }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}