using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.AutoConstructingProviders
{
    internal interface IAutoConstructingProvider
    {
        object AutoResolve(FixtureRequest request, IFixtureContext context);
    }
}
