using System.Linq;
using Sfa.Core.Data;
using Sfa.Core.Entities;
using Sfa.$safeprojectname$.Contexts;

namespace Sfa.$safeprojectname$.Data
{
    public abstract class DomainQuery<TReturn, TQuery> : EntityFrameworkQuery<TReturn, TQuery>
        where TQuery : class, IEntity
    {
        #region Overrides

        protected override IQueryable<TQuery> Queryable => CreateQueryable<TQuery>();

        #endregion


        #region Utilities

        protected IQueryable<T> CreateQueryable<T>()
            where T : class, IEntity
        {
            return DomainContext.Repository.GetQueryable<T>();
        }

        #endregion
    }

    public abstract class DomainQuery<T> : DomainQuery<T, T>
        where T : class, IEntity
    {
    }
}