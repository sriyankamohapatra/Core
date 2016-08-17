using System;
using Sfa.Core.Entities;
using Sfa.MyProject.Contexts;

namespace Sfa.MyProject.Domain
{
    /// <summary>
    /// Represents the base class for all entities that are persisted.
    /// </summary>
    [Serializable]
    public abstract class BaseEntity : BaseDomainObject, IEntity<int>
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected BaseEntity()
            : this(0)
        {

        }

        /// <summary>
        /// Identity constructor
        /// </summary>
        /// <param name="id"></param>
        protected BaseEntity(int id)
        {
            Id = id;
        }

        #endregion


        #region Core Properties

        /// <summary>
        /// The data store id of this instance.
        /// </summary>
        public int Id { get; protected set; }

        #endregion


        #region Life Cycle

        protected void OnCreate()
        {
            DomainContext.Repository.Create(this);
            LogDiagnostics(() => $"Created a {GetType()} with id {Id}");
        }

        protected void OnUpdate()
        {
            DomainContext.Repository.Update(this);
            LogDiagnostics(() => $"Updated a {GetType()} with id {Id}");
        }

        protected void OnDelete()
        {
            DomainContext.Repository.Delete(this);
            LogDiagnostics(() => $"Deleted a {GetType()} with id {Id}");
        }

        #endregion
    }
}