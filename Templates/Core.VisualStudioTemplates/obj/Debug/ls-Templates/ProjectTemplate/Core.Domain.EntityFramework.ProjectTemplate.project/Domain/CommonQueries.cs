using System.Linq;
using Sfa.Core.Entities;
using $safeprojectname$.Data;

namespace $safeprojectname$.Domain
{
    public class CommonQueries
    {
        /// <summary>
        /// Represents a query based upon the id of the instance.
        /// </summary>
        /// <typeparam name="TReturn">The type to return from the execution of the query.</typeparam>
        /// <typeparam name="TQuery">The type to search for.</typeparam>
        public class ForId<TReturn, TQuery> : DomainQuery<TReturn, TQuery>
            where TQuery : class, IEntity<int>
        {
            private readonly int _id;

            public ForId(int id)
            {
                _id = id;
            }

            protected override IQueryable<TQuery> FormLinqQuery(IQueryable<TQuery> query)
            {
                return from items in query
                       where items.Id == _id
                       select items;
            }
        }
    }
}