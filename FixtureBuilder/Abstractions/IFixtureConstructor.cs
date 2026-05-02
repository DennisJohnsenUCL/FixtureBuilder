using FixtureBuilder.Core;
using FixtureBuilder.Creation.UninitializedProviders;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Entry point for building a fixture of type <typeparamref name="T"/>. Inherits from <see cref="IFixtureConfigurator{T}"/> to allow
    /// configuration without explicitly choosing a construction method, using the <see cref="FixtureOptions.DefaultInstantiationMethod"/> instead.
    /// </summary>
    /// <typeparam name="T">The type of the object to construct and configure. Must be a reference type.</typeparam>
    public interface IFixtureConstructor<T> : IFixtureConfigurator<T> where T : class
    {
        /// <summary>
        /// Instantiates the fixture without invoking a constructor, using the <see cref="FixtureOptions.DefaultInitializeMembers"/> option.
        /// </summary>
        /// <returns>An <see cref="IFixtureConfigurator{T}"/> for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<T> CreateUninitialized();

        /// <summary>
        /// Instantiates the fixture without invoking a constructor.
        /// </summary>
        /// <param name="initializeMembers">Controls which members are initialized after instantiation.</param>
        /// <returns>An <see cref="IFixtureConfigurator{T}"/> for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<T> CreateUninitialized(InitializeMembers initializeMembers);

        /// <summary>
        /// Instantiates the fixture using a constructor matching the given arguments.
        /// </summary>
        /// <param name="args">The arguments to pass to the constructor. Must match the constructor's parameter types and order.</param>
        /// <returns>An <see cref="IFixtureConfigurator{T}"/> for further configuration.</returns>
        /// <exception cref="MissingMethodException"/>
        IFixtureConfigurator<T> UseConstructor(params object[] args);

        /// <summary>
        /// Instantiates the fixture by automatically selecting a constructor and resolving all dependencies recursively.
        /// </summary>
        /// <remarks>
        /// Selects the simplest available constructor and resolves each parameter through the fixture's resolution pipeline.
        /// Uses the <see cref="FixtureOptions.PreferSimplestConstructor"/> and <see cref="FixtureOptions.AllowPrivateConstructors"/> options.
        /// </remarks>
        /// <returns>An <see cref="IFixtureConfigurator{T}"/> for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<T> UseAutoConstructor();

        /// <summary>
        /// Instantiates the fixture using a caller-defined factory function.
        /// </summary>
        /// <param name="instantiator">A function that returns the fully constructed instance of <typeparamref name="T"/>.</param>
        /// <returns>An <see cref="IFixtureConfigurator{T}"/> for further configuration.</returns>
        IFixtureConfigurator<T> UseCustomInstantiator(Func<T> instantiator);
    }
}
