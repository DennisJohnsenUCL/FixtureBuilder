using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Creation.AutoConstructingProviders
{
    internal interface IAutoConstructingProvider
    {
        object AutoResolve(TypeRequest request, IFixtureContext context, RecursiveResolveContext? recursiveResolveContext = null);
    }
}
