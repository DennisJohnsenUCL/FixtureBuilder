using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.FixtureProviders.Providers
{
    internal class StringProvider : IFixtureProvider
    {
        public object? Resolve(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type == typeof(string))
            {
                if (request.Name != null) return request.Name;
                return "";
            }

            return null;
        }
    }
}
