using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.FixtureProviders.Providers
{
    internal class DefaultBclTypeProvider : IFixtureProvider
    {
        public object? Resolve(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type.Namespace?.StartsWith("System") == true)
            {
                return Activator.CreateInstance(request.Type);
            }

            return null;
        }
    }
}
