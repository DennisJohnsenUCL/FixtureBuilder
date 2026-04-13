using FixtureBuilder.Creation.UninitializedProviders;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public interface IConstructor<T>
    {
        T CreateUninitialized();
        T CreateUninitialized(InitializeMembers initializeMembers);
        T UseConstructor(params object[] args);
        T UseAutoConstructor();
    }
}
