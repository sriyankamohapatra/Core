namespace Sfa.Core.Data
{
    /// <summary>
    /// Represents a command that is a unit of work.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        void Execute();

        /// <summary>
        /// Checks to see if the command is authorised.
        /// </summary>
        /// <returns><c>true</c> if the command is authorised, otherwise, <c>false</c>.</returns>
        bool Authorise();
    }
}