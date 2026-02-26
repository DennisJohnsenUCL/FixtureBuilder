using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.FixtureProviders.Providers
{
    internal class GuidProvider : IFixtureProvider
    {
        public object? Resolve(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type == typeof(Guid))
            {
                return Guid.NewGuid();
            }

            return null;
        }
    }
}
