using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.UninitializedProviders
{
    internal interface IFixtureUninitializedProvider
    {
        object? ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context);
    }
}
