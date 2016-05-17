using System;

namespace Sfa.Core.Equality
{
    /// <summary>
    /// Decorates fields that are not to participate in the value comparison of their owning object.
    /// If put on a class then standard equality is used on that class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class NoValueCompareAttribute : Attribute
    {
    }
}