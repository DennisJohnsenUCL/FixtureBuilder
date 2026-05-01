using FixtureBuilder.Core;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Entry point for building a fixture of type <typeparamref name="T"/>. Inherits from <see cref="IFixtureConfigurator{T}"/> to allow
    /// configuration without explicitly choosing a construction method, using the <see cref="FixtureOptions.DefaultInstantiationMethod"/> instead.
    /// </summary>
    /// <typeparam name="T">The type of the object to construct and configure. Must be a reference type.</typeparam>
    public interface IFixtureConstructor<T> : IConstructor<IFixtureConfigurator<T>>, IFixtureConfigurator<T> where T : class;
}
