using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.UninitializedProviders
{
    internal interface IUninitializedProvider
    {
        object? ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context, RecursiveResolveContext? recursiveResolveContext = null);
    }
}
