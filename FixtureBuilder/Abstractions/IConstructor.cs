using FixtureBuilder.Core;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Provides construction methods for creating an instance, returning <typeparamref name="TReturn"/> as the result.
    /// </summary>
    /// <typeparam name="TReturn">The return type of the construction methods.</typeparam>
    public interface IConstructor<TReturn>
    {
        /// <summary>
        /// Creates an uninitialized instance without invoking a constructor, using <see cref="InitializeMembers.None"/>.
        /// </summary>
        /// <returns>The result of type <typeparamref name="TReturn"/>.</returns>
        /// <exception cref="InvalidOperationException"/>
        TReturn CreateUninitialized();

        /// <summary>
        /// Creates an uninitialized instance without invoking a constructor.
        /// </summary>
        /// <param name="initializeMembers">Controls which members are initialized after instantiation.</param>
        /// <returns>The result of type <typeparamref name="TReturn"/>.</returns>
        /// <exception cref="InvalidOperationException"/>
        TReturn CreateUninitialized(InitializeMembers initializeMembers);

        /// <summary>
        /// Creates an instance using a constructor matching the given arguments.
        /// </summary>
        /// <param name="args">The arguments to pass to the constructor. Must match the constructor's parameter types and order.</param>
        /// <returns>The result of type <typeparamref name="TReturn"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException"/>
        TReturn UseConstructor(params object?[] args);

        /// <summary>
        /// Creates an instance by automatically selecting a constructor and resolving all dependencies recursively.
        /// </summary>
        /// <remarks>
        /// Selects the simplest available constructor and resolves each parameter through the fixture's resolution pipeline.
        /// Uses the <see cref="FixtureOptions.PreferSimplestConstructor"/> and <see cref="FixtureOptions.AllowPrivateConstructors"/> options.
        /// Constructors with a parameter of the type being built, after type links are applied, are tried last, and skipped unless the resolution pipeline
        /// provides a value for that parameter, for example null when it is nullable and <see cref="FixtureOptions.PreferNullParameterValues"/> is enabled,
        /// its default value when it has one and <see cref="FixtureOptions.PreferDefaultParameterValues"/> is enabled, or a value from a registered provider.
        /// </remarks>
        /// <returns>The result of type <typeparamref name="TReturn"/>.</returns>
        /// <exception cref="InvalidOperationException"/>
        TReturn UseAutoConstructor();

        /// <summary>
        /// Creates an instance using a caller-defined factory function.
        /// </summary>
        /// <param name="instantiator">A function that returns the fully constructed instance.</param>
        /// <returns>The result of type <typeparamref name="TReturn"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instantiator"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException"/>
        TReturn UseCustomInstantiator(Func<TReturn> instantiator);
    }
}
