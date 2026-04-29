using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Bogus
{
    public interface IBogusFixtureConstructor<T> : IBogusFixtureConfigurator<T> where T : class
    {
        IBogusFixtureConfigurator<T> CreateUninitialized();

        IBogusFixtureConfigurator<T> CreateUninitialized(InitializeMembers initializeMembers);

        IBogusFixtureConfigurator<T> UseConstructor(params object[] args);

        IBogusFixtureConfigurator<T> UseAutoConstructor();
    }
}
