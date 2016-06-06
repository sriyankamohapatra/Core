using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public abstract class Command<TTarget, TResult> : ICommand
    {
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
        public bool Authorise()
        {
            return OnAuthorise();
        }

        /// <summary>
        /// Executes the command, performing initialisation of the target and authorisation.
        /// Implementers can override this method or the individual pieces that make up
        /// the execution steps.
        /// </summary>
        /// <exception cref="UnauthorizedException">Thrown when the command isn't authorised to be executed.</exception>
        public virtual void Execute()
        {
            OnBeforeInitialiseTarget();
            OnInitialiseTarget();

            if (!OnAuthorise())
            {
                throw new UnauthorizedException();
            }

            OnBeforeExecute();
            try
            {
                OnExecute();
                OnAfterExecute();
            }
            catch (Exception exception)
            {
                OnAfterExecute(exception);
                throw;
            }
        }

        /// <summary>
        /// Should be implemented as the actual execution step of the command.
        /// </summary>
        protected abstract void OnExecute();

        /// <summary>
        /// If overridden, this step is called before the initialisation of the target.
        /// </summary>
        protected virtual void OnBeforeInitialiseTarget()
        {
        }

        /// <summary>
        /// Override this to set up the target of the command.
        /// </summary>
        protected virtual void OnInitialiseTarget()
        {
        }


        /// <summary>
        /// Override this to implement any domain specific authorisation
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnAuthorise()
        {
            return true;
        }

        /// <summary>
        /// Override this to perform any custom actions once the command has been executed.
        /// </summary>
        /// <param name="exception">Any exception thrown during the execution of the command.</param>
        protected virtual void OnAfterExecute(Exception exception = null)
        {
        }

        /// <summary>
        /// Called before <see cref="OnExecute"/> but after <see cref="OnInitialiseTarget"/>.
        /// Override this to implement any custom behaviour.
        /// </summary>
        protected virtual void OnBeforeExecute()
        {
        }

        #region Overrides

        /// <summary>
        /// Gives a string representation of the commands properties.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var stringRepresentation = new StringBuilder();
            var properties = GetType().GetProperties();

            foreach (var propertyInfo in properties)
            {
                if (!PropertiesToIgnoreForToString.Contains(propertyInfo.Name))
                {
                    stringRepresentation.AppendFormat("[{0}:{1}] ", propertyInfo.Name, propertyInfo.GetValue(this));
                }
            }

            return stringRepresentation.ToString().Trim();
        }

        /// <summary>
        /// Lists the names of properties that shouldn't be included within the ToString representation.
        /// </summary>
        protected virtual IEnumerable<string> PropertiesToIgnoreForToString
        {
            get
            {
                yield return nameof(Target);
                yield return nameof(Result);
            }
        }

        #endregion
    }
}