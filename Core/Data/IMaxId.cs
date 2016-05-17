using Sfa.Core.Entities;

namespace Sfa.Core.Data
{
    /// <summary>
    /// API for querying the maximum id of an Entity.
    /// </summary>
    public interface IMaxEntityId
    {
        /// <summary>
        /// Returns the Max Id for the Entity type specified.
        /// </summary>
        /// <typeparam name="T">The type of the Entity to get the Max id for.</typeparam>
        /// <typeparam name="TId">The type of the id.</typeparam>
        /// <returns>The maximum id for the entity type.</returns>
        TId GetMaxId<T, TId>()
             where T : class, IEntity<TId>;
    }
}