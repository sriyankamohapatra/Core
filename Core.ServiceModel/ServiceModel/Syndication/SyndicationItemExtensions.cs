using System;
using System.ServiceModel.Syndication;

namespace Sfa.Core.ServiceModel.Syndication
{
    public static class SyndicationItemExtensions
    {
        public static Guid IdAsGuid(this SyndicationItem item)
        {
            return Guid.Parse(item.Id.StartsWith("uuid:")
                ? item.Id.Substring(5)
                : item.Id);
        }
    }
}