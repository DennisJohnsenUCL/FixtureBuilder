using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for Base Class Library (BCL) types in the <c>System</c> namespace hierarchy.
    /// </summary>
    /// <remarks>
    /// Acts as a fallback provider for types whose namespace starts with <c>System</c>,
    /// creating instances via their parameterless constructor using <see cref="Activator.CreateInstance(Type)"/>.
    /// Returns <see langword="null"/> for types outside the <c>System</c> namespace or those without a namespace.
    /// </remarks>
    internal class DefaultBclTypeProvider : IValueProvider
    {
        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type.Namespace?.StartsWith("System") == true)
            {
                try
                {
                    return Activator.CreateInstance(request.Type);
                }
                catch
                {
                    return new NoResult();
                }
            }

            return new NoResult();
        }
    }
}
