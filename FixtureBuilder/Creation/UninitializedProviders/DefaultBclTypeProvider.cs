using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Creation.UninitializedProviders
{
    /// <summary>
    /// Provides fixture instances for Base Class Library (BCL) types in the <c>System</c> namespace hierarchy.
    /// </summary>
    /// <remarks>
    /// Acts as a fallback provider for types whose namespace starts with <c>System</c>,
    /// creating instances via their parameterless constructor using <see cref="Activator.CreateInstance(Type)"/>.
    /// Returns <see cref="NoResult"/> for types outside the <c>System</c> namespace or those without a namespace,
    /// and for types that cannot be created this way.
    /// </remarks>
    internal class DefaultBclTypeProvider : IValueProvider
    {
        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            var ns = request.Type.Namespace;

            if (ns == "System" || ns?.StartsWith("System.") == true)
            {
                try
                {
                    return Activator.CreateInstance(request.Type);
                }
                catch
                {
                    return NoResult.Instance;
                }
            }

            return NoResult.Instance;
        }
    }
}
