#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public interface IFixtureConstructor<T> : IConstructor<IFixtureConfigurator<T>>, IFixtureConfigurator<T> where T : class;
}
