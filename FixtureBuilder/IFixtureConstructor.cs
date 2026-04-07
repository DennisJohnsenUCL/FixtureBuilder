using FixtureBuilder.UninitializedProviders;

namespace FixtureBuilder
{
    public interface IFixtureConstructor<T> : IFixtureConfigurator<T> where T : class
    {
        IFixtureConfigurator<T> CreateUninitialized();
        IFixtureConfigurator<T> CreateUninitialized(InitializeMembers initializeMembers);
        IFixtureConfigurator<T> UseConstructor(params object[] args);
        IFixtureConfigurator<T> UseAutoConstructor();
    }
}
