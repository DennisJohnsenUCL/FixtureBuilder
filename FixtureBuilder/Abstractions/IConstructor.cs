using FixtureBuilder.Core;
using FixtureBuilder.Creation.UninitializedProviders;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public interface IConstructor<TReturn>
    {
        /// <summary>
        /// Creates an uninitialized instance without invoking a constructor, using the <see cref="FixtureOptions.DefaultInitializeMembers"/> option.
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
        /// <exception cref="MissingMethodException"/>
        TReturn UseConstructor(params object[] args);

        /// <summary>
        /// Creates an instance by automatically selecting a constructor and resolving all dependencies recursively.
        /// </summary>
        /// <remarks>
        /// Selects the simplest available constructor and resolves each parameter through the fixture's resolution pipeline.
        /// Uses the <see cref="FixtureOptions.PreferSimplestConstructor"/> and <see cref="FixtureOptions.AllowPrivateConstructors"/> options.
        /// </remarks>
        /// <returns>The result of type <typeparamref name="TReturn"/>.</returns>
        /// <exception cref="InvalidOperationException"/>
        TReturn UseAutoConstructor();
    }
}
