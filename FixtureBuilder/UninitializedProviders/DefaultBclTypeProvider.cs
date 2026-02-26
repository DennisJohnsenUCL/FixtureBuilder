using FixtureBuilder.FixtureContexts;
using FixtureBuilder.FixtureProviders;

namespace FixtureBuilder.UninitializedProviders
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
