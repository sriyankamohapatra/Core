using System;
using System.Threading.Tasks;

namespace Sfa.Core.Threading
{
    /// <summary>
    ///     Extension available of the <see cref="Task" /> class.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        ///     Gets the name of the property.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>The type of the object.</returns>
        public static T GetSafeResult<T>(this Task<T> task)
        {
            try
            {
                return task.Result;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                throw;
            }
        }
               
    }
}