using System;

namespace Sfa.Core
{
    /// <summary>Enum Attribute.</summary>
    /// <remarks>AttributeTargets set to "All", there is no setting for enum values</remarks>
    [AttributeUsage(AttributeTargets.All)]
    public class EnumDisplayAttribute : EnumDescriptorAttribute
    {
        public string DisplayText { get; set; }
    }
}