using System;
using System.ServiceModel.Syndication;

namespace Sfa.Core.ServiceModel.Syndication
{
    /// <summary>
    /// Syndication item extension class.
    /// </summary>
    public static class SyndicationItemExtensions
    {
        /// <summary>
        /// Gets the Id for the Item when the Id is a Guid.
        /// </summary>
        /// <param name="item">The syndication item.</param>
        /// <returns></returns>
        public static Guid IdAsGuid(this SyndicationItem item)
        {
            return Guid.Parse(item.Id.StartsWith("uuid:")
                ? item.Id.Substring(5)
                : item.Id);
        }
    }
}