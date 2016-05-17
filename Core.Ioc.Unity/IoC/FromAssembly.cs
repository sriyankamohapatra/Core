using System.Reflection;

namespace Sfa.Core.IoC
{
    /// <summary>
    /// Helper class to help with fluent method chaining.
    /// </summary>
    public static class FromAssembly
    {
        /// <summary>
        /// Returns the assembly that the supplied type is defined in.
        /// </summary>
        /// <typeparam name="T">The type to find the assembly of.</typeparam>
        /// <returns>The assembly for the supplied type.</returns>
        public static Assembly Containing<T>()
        {
            return typeof(T).Assembly;
        }
    }
}