using System;
using Sfa.Core.Context;
using Sfa.Core.Data;
using Sfa.Core.Exceptions;
using Sfa.Core.Logging;
using Sfa.$safeprojectname$.Contexts;

namespace Sfa.$safeprojectname$.Data
{
    /// <summary>
    /// Base class for all commands in the domain.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target instance.</typeparam>
    /// <typeparam name="TResult">The type of the result instance.</typeparam>
    public abstract class DomainCommand<TTarget, TResult> : Command<TTarget, TResult>, IDomainCommand
    {
        /// <summary>
        /// Determines if this command's execution path should run inside a transaction.
        /// </summary>
        public virtual bool ExecutionRequiresTransaction { get; protected set; }

        protected override void OnBeforeExecute()
        {
            base.OnBeforeExecute();
            var transRepo = DomainContext.Repository as ITransactional;
            if (ExecutionRequiresTransaction)
            {
                transRepo?.BeginTransaction();
            }
        }

        /// <summary>
        /// Fires after the command has executed successfully.
        /// </summary>
        /// <param name="exception">Any exception thrown during the execution of the command.</param>
        /// <returns>The continuation task.</returns>
        protected override void OnAfterExecute(Exception exception = null)
        {
            var transRepo = DomainContext.Repository as ITransactional;

            var businessLogicException = exception as BusinessLogicException;
            if (exception == null || (businessLogicException != null && businessLogicException.TreatAsCommitSuccess))
            {
                DomainContext.Repository.SaveChanges();

                if (transRepo != null && transRepo.HasCurrenTransaction)
                {
                    transRepo.CommitTransaction();
                }
            }
            else
            {
                if (transRepo != null && transRepo.HasCurrenTransaction)
                {
                    ApplicationContext.Logger.Log(LoggingLevel.Error, CoreLoggingCategory.Diagnostics, () => "Rolling back transaction because of the exception for {0}. We also log the exception in its entirety following this log.", exception.Message);
                    ApplicationContext.Logger.LogException(CoreLoggingCategory.Diagnostics, exception);

                    transRepo.RollbackTransaction();
                }
            }
        }
    }

    /// <summary>
    /// Base class for all commands in the domain.
    /// </summary>
    /// <typeparam name="TResult">The type of the result instance.</typeparam>
    public abstract class DomainCommand<TResult> : DomainCommand<TResult, TResult>
    {

    }
}