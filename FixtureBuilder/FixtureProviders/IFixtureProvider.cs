using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.FixtureProviders
{
    internal interface IFixtureProvider
    {
        object? Resolve(FixtureRequest request, IFixtureContext context);
    }
}
