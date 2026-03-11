using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.FixtureProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for <see cref="DateTime"/> types.
    /// </summary>
    /// <remarks>
    /// Returns <see cref="DateTime.UtcNow"/> for requests matching <see cref="DateTime"/>;
    /// returns <see langword="null"/> for all other types.
    /// </remarks>
    internal class DateTimeProvider : IFixtureProvider
    {
        public object? Resolve(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type == typeof(DateTime))
            {
                return DateTime.UtcNow;
            }

            return null;
        }
    }
}
