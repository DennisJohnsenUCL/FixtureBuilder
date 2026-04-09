using FixtureBuilder.UninitializedProviders;

namespace FixtureBuilder
{
    public interface IConstructor<T>
    {
        T CreateUninitialized();
        T CreateUninitialized(InitializeMembers initializeMembers);
        T UseConstructor(params object[] args);
        T UseAutoConstructor();
    }
}
