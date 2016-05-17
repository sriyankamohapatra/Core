using System.Data.Entity;

namespace Sfa.Core.Data.Folder
{
    /// <summary>
    /// Extends the <see cref="DbSet{T}"/> class.
    /// </summary>
    public static class DbSetExtensions
    {
        /// <summary>
        /// Clears all data from a set.
        /// </summary>
        /// <typeparam name="T">The type of entity held in the set.</typeparam>
        /// <param name="dbSet">The set to clear,</param>
        public static void Clear<T>(this DbSet<T> dbSet)
            where T : class
        {
            dbSet.RemoveRange(dbSet);
        }
    }
}