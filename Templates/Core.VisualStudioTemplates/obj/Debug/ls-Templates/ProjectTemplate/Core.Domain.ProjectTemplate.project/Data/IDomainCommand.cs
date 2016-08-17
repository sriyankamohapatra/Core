namespace $safeprojectname$.Data
{
    /// <summary>
    /// Adds domain specific aspects to the command interface.
    /// </summary>
    public interface IDomainCommand
    {
        bool ExecutionRequiresTransaction { get; }
    }
}