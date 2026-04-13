using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Creation.AutoConstructingProviders
{
    internal interface IAutoConstructingProvider
    {
        object AutoResolve(FixtureRequest request, IFixtureContext context, RecursiveResolveContext? recursiveResolveContext = null);
    }
}
