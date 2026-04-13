using FixtureBuilder.Core;

namespace FixtureBuilder.Creation.ConstructingProviders
{
    internal interface IConstructingProvider
    {
        object ResolveWithArguments(FixtureRequest request, params object?[] args);
    }
}
