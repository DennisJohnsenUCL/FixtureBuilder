using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.FixtureProviders.Providers
{
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
