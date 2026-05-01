using Bogus;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Bogus
{
    /// <summary>
    /// Provides construction methods for instantiating a member of type <typeparamref name="TReturn"/>,
    /// with optional <see cref="Faker"/>-based data generation.
    /// </summary>
    /// <typeparam name="TReturn">The type of the member to instantiate.</typeparam>
    public interface IBogusConstructor<TReturn>
    {
        /// <summary>
        /// Creates an instance using a constructor whose arguments are resolved via a <see cref="Faker"/> lambda.
        /// </summary>
        /// <param name="args">A function that receives a <see cref="Faker"/> and returns the arguments to pass to the constructor.</param>
        /// <returns>The instantiated member of type <typeparamref name="TReturn"/>.</returns>
        /// <exception cref="MissingMethodException"/>
        TReturn UseConstructor(Func<Faker, object[]> args);

        /// <summary>
        /// Creates an uninitialized instance without invoking a constructor, using the <see cref="FixtureOptions.DefaultInitializeMembers"/> option.
        /// </summary>
        /// <returns>The instantiated member of type <typeparamref name="TReturn"/>.</returns>
        /// <exception cref="InvalidOperationException"/>
        TReturn CreateUninitialized();

        /// <summary>
        /// Creates an uninitialized instance without invoking a constructor.
        /// </summary>
        /// <param name="initializeMembers">Controls which members are initialized after instantiation.</param>
        /// <returns>The instantiated member of type <typeparamref name="TReturn"/>.</returns>
        /// <exception cref="InvalidOperationException"/>
        TReturn CreateUninitialized(InitializeMembers initializeMembers);

        /// <summary>
        /// Creates an instance using a constructor matching the given arguments.
        /// </summary>
        /// <param name="args">The arguments to pass to the constructor. Must match the constructor's parameter types and order.</param>
        /// <returns>The instantiated member of type <typeparamref name="TReturn"/>.</returns>
        /// <exception cref="MissingMethodException"/>
        TReturn UseConstructor(params object[] args);

        /// <summary>
        /// Creates an instance by automatically selecting a constructor and resolving all dependencies recursively.
        /// </summary>
        /// <returns>The instantiated member of type <typeparamref name="TReturn"/>.</returns>
        /// <exception cref="InvalidOperationException"/>
        TReturn UseAutoConstructor();

        /// <summary>
        /// Creates an instance using a caller-defined factory that receives a <see cref="Faker"/> for data generation.
        /// </summary>
        /// <param name="factory">A function that receives a <see cref="Faker"/> and returns the fully constructed instance of <typeparamref name="TReturn"/>.</param>
        /// <returns>The instantiated member of type <typeparamref name="TReturn"/>.</returns>
        TReturn UseCustomInstantiator(Func<Faker, TReturn> factory);
    }
}
