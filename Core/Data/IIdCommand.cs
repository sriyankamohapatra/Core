namespace Sfa.Core.Data
{
    /// <summary>
    /// Represents a command, where there is an ID associated with the flow of the execution of it.
    /// Usually used when the Target has an id and can be loaded from a data store and the command
    /// is bound using convention.
    /// </summary>
    /// <typeparam name="TId">The type of the id.</typeparam>
    public interface IIdCommand<TId> : ICommand
    {
        TId Id { get; set; }
    }
}