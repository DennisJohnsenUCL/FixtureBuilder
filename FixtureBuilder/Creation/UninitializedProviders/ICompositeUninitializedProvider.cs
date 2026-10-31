using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Creation.UninitializedProviders
{
    internal interface ICompositeUninitializedProvider
    {
        public object? ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context, RecursiveResolveContext recursiveResolveContext);
    }
}
