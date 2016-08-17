using System;
using System.ComponentModel.DataAnnotations;
using Sfa.Core.Data;
using Sfa.Core.Entities;
using Sfa.Core.Exceptions;
using $safeprojectname$.Contexts;

namespace $safeprojectname$.Data
{
    public abstract class DomainIdCommand<T, TId> : DomainIdCommand<T, TId, T>
        where T : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
    }

    public abstract class DomainIdCommand<TTarget, TTargetId, TResult> : DomainCommand<TTarget, TResult>, IIdCommand<TTargetId>
        where TTarget : class, IEntity<TTargetId>
        where TTargetId : IEquatable<TTargetId>
    {
        [Required]
        public virtual TTargetId Id { get; set; }

        protected override void OnInitialiseTarget()
        {
            OnLoadTarget();

            if (Target == null)
            {
                throw new MissingEntityException(typeof(TTarget));
            }
        }

        protected virtual void OnLoadTarget()
        {
            Target = DomainContext.Repository.Load<TTarget, TTargetId>(Id);
        }
    }
}