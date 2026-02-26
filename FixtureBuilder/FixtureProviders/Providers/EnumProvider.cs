using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.FixtureProviders.Providers
{
    internal class EnumProvider : IFixtureProvider
    {
        public object? Resolve(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type.IsEnum)
            {
                return Enum.GetValues(request.Type).GetValue(0);
            }

            return null;
        }
    }
}
