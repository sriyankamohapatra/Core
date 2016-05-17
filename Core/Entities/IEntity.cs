namespace Sfa.Core.Entities
{
    /// <summary>
    /// Represents a domain entity.
    /// </summary>
    public interface IEntity
    {

    }

    /// <summary>
    /// Represents a domain entity that has an identity.
    /// </summary>
    /// <typeparam name="T">The type of the id of the entity.</typeparam>
    public interface IEntity<out T> : IEntity
    {
        T Id { get; }
    }
}