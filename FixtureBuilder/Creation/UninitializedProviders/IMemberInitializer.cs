using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Creation.UninitializedProviders
{
    internal interface IMemberInitializer
    {
        void InitializeMembers(object instance, InitializeMembers initializeMembers, IFixtureContext context, RecursiveResolveContext recursiveResolveContext);
    }
}
