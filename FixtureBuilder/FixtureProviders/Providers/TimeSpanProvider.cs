using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.FixtureProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for <see cref="TimeSpan"/> types.
    /// </summary>
    /// <remarks>
    /// Returns a fixed <see cref="TimeSpan"/> of 30 minutes for each request.
    /// Returns <see langword="null"/> for all other types.
    /// </remarks>
    internal class TimeSpanProvider : IFixtureProvider
    {
        public object? Resolve(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type == typeof(TimeSpan))
            {
                return TimeSpan.FromMinutes(30);
            }

            return null;
        }
    }
}
