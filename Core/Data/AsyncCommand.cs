using System;
using System.Threading.Tasks;
using Sfa.Core.Exceptions;

namespace Sfa.Core.Data
{
    /// <summary>
    /// Abstracts a unit of work into a single command that can encapsulate authorisation and validation
    /// in a consistent way.
    /// </summary>
    /// <typeparam name="TTarget">The target to apply the command on.</typeparam>
    /// <typeparam name="TResult">The result type of executing the command. Its up to the implementer
    /// to set the result.</typeparam>
    public abstract class AsyncCommand<TTarget, TResult> : IAsyncCommand
    {
        #region Main Api

        /// <summary>
        /// The target of the command.
        /// </summary>
        public TTarget Target { get; set; }

        /// <summary>
        /// The result from executing the command.
        /// </summary>
        public TResult Result { get; set; }

        /// <summary>
        /// <c>true</c> if the command is authorised to be executed.
        /// </summary>
        /// <returns><c>true</c> if authorised; otherwise, <c>false</c>.</returns>
        public async Task<bool> AuthoriseAsync()
        {
            return await OnAuthoriseAsync();
        }

        /// <summary>
        /// Executes the command, performing initialisation of the target and authorisation.
        /// Implementers can override this method or the individual pieces that make up
        /// the execution steps.
        /// </summary>
        /// <exception cref="UnauthorizedException">Thrown when the command isn't authorised to be executed.</exception>
        public virtual async Task ExecuteAsync()
        {
            await OnBeforeInitialiseTargetAsync();
            await OnInitialiseTargetAsync();

            if (!(await OnAuthoriseAsync()))
            {
                throw new UnauthorizedException();
            }

            await OnBeforeExecuteAsync();
            try
            {
                await OnExecuteAsync();
                await OnAfterExecuteAsync();
            }
            catch (Exception exception)
            {
                await OnAfterExecuteAsync(exception);
                throw;
            }
        }

        #endregion


        #region Internal Implementations
        
        /// <summary>
        /// Should be implemented as the actual execution step of the command.
        /// </summary>
        protected abstract Task OnExecuteAsync();

        /// <summary>
        /// If overridden, this step is called before the initialisation of the target.
        /// </summary>
        protected virtual Task OnBeforeInitialiseTargetAsync()
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Override this to set up the target of the command.
        /// </summary>
        protected virtual Task OnInitialiseTargetAsync()
        {
            return Task.FromResult(0);
        }


        /// <summary>
        /// Override this to implement any domain specific authorisation
        /// </summary>
        /// <returns></returns>
        protected virtual Task<bool> OnAuthoriseAsync()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Override this to perform any custom actions once the command has been executed.
        /// </summary>
        /// <param name="exception">Any exception thrown during the execution of the command.</param>
        protected virtual Task OnAfterExecuteAsync(Exception exception = null)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Called before <see cref="OnExecuteAsync"/> but after <see cref="OnInitialiseTargetAsync"/>.
        /// Override this to implement any custom behaviour.
        /// </summary>
        protected virtual Task OnBeforeExecuteAsync()
        {
            return Task.FromResult(0);
        }

        #endregion
    }
}