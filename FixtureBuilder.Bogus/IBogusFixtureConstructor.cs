using Bogus;
using FixtureBuilder.Core;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Bogus
{
    /// <summary>
    /// Provides construction methods for creating a Bogus-integrated fixture of type <typeparamref name="T"/>.
    /// Choose a construction method to control how the fixture is instantiated, then configure it via the returned <see cref="IBogusFixtureConfigurator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to construct. Must be a reference type.</typeparam>
    public interface IBogusFixtureConstructor<T> : IBogusFixtureConfigurator<T> where T : class
    {
        #region IBogusFixtureConstructor

        /// <summary>
        /// Creates an instance using a constructor whose arguments are resolved via a <see cref="Faker"/> lambda.
        /// </summary>
        /// <param name="args">A function that receives a <see cref="Faker"/> and returns the arguments to pass to the constructor.</param>
        /// <returns>An <see cref="IBogusFixtureConfigurator{T}"/> for further configuration.</returns>
        /// <exception cref="MissingMethodException"/>
        IBogusFixtureConfigurator<T> UseConstructor(Func<Faker, object[]> args);

        /// <summary>
        /// Creates an instance using a caller-defined factory that receives a <see cref="Faker"/> for data generation.
        /// </summary>
        /// <param name="factory">A function that receives a <see cref="Faker"/> and returns the fully constructed instance of <typeparamref name="T"/>.</param>
        /// <returns>An <see cref="IBogusFixtureConfigurator{T}"/> for further configuration.</returns>
        IBogusFixtureConfigurator<T> UseCustomInstantiator(Func<Faker, T> factory);

        #endregion

        #region IFixtureConstructor

        /// <summary>
        /// Creates an uninitialized instance without invoking a constructor, using the <see cref="FixtureOptions.DefaultInitializeMembers"/> option.
        /// </summary>
        /// <returns>An <see cref="IBogusFixtureConfigurator{T}"/> for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IBogusFixtureConfigurator<T> CreateUninitialized();

        /// <summary>
        /// Creates an uninitialized instance without invoking a constructor.
        /// </summary>
        /// <param name="initializeMembers">Controls which members are initialized after instantiation.</param>
        /// <returns>An <see cref="IBogusFixtureConfigurator{T}"/> for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IBogusFixtureConfigurator<T> CreateUninitialized(InitializeMembers initializeMembers);

        /// <summary>
        /// Creates an instance using a constructor matching the given arguments.
        /// </summary>
        /// <param name="args">The arguments to pass to the constructor. Must match the constructor's parameter types and order.</param>
        /// <returns>An <see cref="IBogusFixtureConfigurator{T}"/> for further configuration.</returns>
        /// <exception cref="MissingMethodException"/>
        IBogusFixtureConfigurator<T> UseConstructor(params object[] args);

        /// <summary>
        /// Creates an instance by automatically selecting a constructor and resolving all dependencies recursively.
        /// </summary>
        /// <remarks>
        /// Selects the simplest available constructor and resolves each parameter through the fixture's resolution pipeline.
        /// Uses the <see cref="FixtureOptions.PreferSimplestConstructor"/> and <see cref="FixtureOptions.AllowPrivateConstructors"/> options.
        /// </remarks>
        /// <returns>An <see cref="IBogusFixtureConfigurator{T}"/> for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IBogusFixtureConfigurator<T> UseAutoConstructor();

        /// <summary>
        /// Instantiates the fixture using a caller-defined factory function.
        /// </summary>
        /// <param name="instantiator">A function that returns the fully constructed instance of <typeparamref name="T"/>.</param>
        /// <returns>An <see cref="IBogusFixtureConfigurator{T}"/> for further configuration.</returns>
        IBogusFixtureConfigurator<T> UseCustomInstantiator(Func<T> instantiator);

        #endregion
    }
}
