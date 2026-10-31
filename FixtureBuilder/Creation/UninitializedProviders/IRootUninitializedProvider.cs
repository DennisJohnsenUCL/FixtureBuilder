using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Creation.UninitializedProviders
{
    internal interface IRootUninitializedProvider
    {
        object ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context, RecursiveResolveContext? recursiveResolveContext = null);
    }
}
