using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for <see cref="Guid"/> types.
    /// </summary>
    /// <remarks>
    /// Returns a new unique <see cref="Guid"/> generated via <see cref="Guid.NewGuid"/> for each request.
    /// Returns <see langword="null"/> for all other types.
    /// </remarks>
    internal class GuidProvider : IValueProvider
    {
        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type == typeof(Guid))
            {
                return Guid.NewGuid();
            }

            return null;
        }
    }
}
