using System.Threading.Tasks;

namespace Sfa.Core.Threading
{
    /// <summary>
    /// Extensions for <see cref="Task" />.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Gets the result of the <see cref="Task"/>, but handles where a <see cref="TaskCanceledException"/> is thrown 
        /// and there is an inner exception. In this scenario, the inner exception is the exception that is thrown.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>The result of running the <see cref="Task"/>.</returns>
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