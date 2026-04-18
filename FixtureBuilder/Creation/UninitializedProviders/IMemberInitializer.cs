using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Creation.UninitializedProviders
{
    internal interface IMemberInitializer
    {
        void InitializeMembers(object instance, InitializeMembers initializeMembers, Type rootType, IFixtureContext context, RecursiveResolveContext recursiveResolveContext);
    }
}
