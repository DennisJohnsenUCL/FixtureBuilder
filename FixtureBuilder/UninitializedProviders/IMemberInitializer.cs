using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.UninitializedProviders
{
    internal interface IMemberInitializer
    {
        void InitializeMembers(object instance, InitializeMembers initializeMembers, IFixtureContext context);
    }
}
