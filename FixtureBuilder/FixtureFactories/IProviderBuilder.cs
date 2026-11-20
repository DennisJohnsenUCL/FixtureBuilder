namespace FixtureBuilder.FixtureFactories
{
    /// <summary>
    /// Provides fluent methods for registering values and factory functions to be used during value resolution, scoped by member kind and name.
    /// </summary>
    /// <typeparam name="TReturn">The return type for fluent chaining.</typeparam>
    internal interface IProviderBuilder<TReturn>
    {
        /// <summary>
        /// Registers a value to be used for all members and parameters of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// The same instance is provided on every resolution, for every fixture this registration applies to.
        /// If <typeparamref name="T"/> is a mutable reference type, configuring members of the provided instance changes it for every fixture that receives it.
        /// Use the <c>Func&lt;T&gt;</c> overload to provide a new instance per resolution.
        /// </remarks>
        TReturn With<T>(T value);

        /// <summary>
        /// Registers a factory function to be invoked for all members and parameters of type <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
        TReturn With<T>(Func<T> func);

        /// <summary>
        /// Registers a value to be used for members and parameters of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        /// <remarks>
        /// The same instance is provided on every resolution, for every fixture this registration applies to.
        /// If <typeparamref name="T"/> is a mutable reference type, configuring members of the provided instance changes it for every fixture that receives it.
        /// Use the <c>Func&lt;T&gt;</c> overload to provide a new instance per resolution.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        TReturn With<T>(T value, string name);

        /// <summary>
        /// Registers a factory function to be invoked for members and parameters of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
        TReturn With<T>(Func<T> func, string name);

        /// <summary>
        /// Registers a value to be used for constructor parameters of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// The same instance is provided on every resolution, for every fixture this registration applies to.
        /// If <typeparamref name="T"/> is a mutable reference type, configuring members of the provided instance changes it for every fixture that receives it.
        /// Use the <c>Func&lt;T&gt;</c> overload to provide a new instance per resolution.
        /// </remarks>
        TReturn WithParameter<T>(T value);

        /// <summary>
        /// Registers a factory function to be invoked for constructor parameters of type <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
        TReturn WithParameter<T>(Func<T> func);

        /// <summary>
        /// Registers a value to be used for constructor parameters of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        /// <remarks>
        /// The same instance is provided on every resolution, for every fixture this registration applies to.
        /// If <typeparamref name="T"/> is a mutable reference type, configuring members of the provided instance changes it for every fixture that receives it.
        /// Use the <c>Func&lt;T&gt;</c> overload to provide a new instance per resolution.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        TReturn WithParameter<T>(T value, string name);

        /// <summary>
        /// Registers a factory function to be invoked for constructor parameters of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
        TReturn WithParameter<T>(Func<T> func, string name);

        /// <summary>
        /// Registers a value to be used for properties and fields of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// The same instance is provided on every resolution, for every fixture this registration applies to.
        /// If <typeparamref name="T"/> is a mutable reference type, configuring members of the provided instance changes it for every fixture that receives it.
        /// Use the <c>Func&lt;T&gt;</c> overload to provide a new instance per resolution.
        /// </remarks>
        TReturn WithPropertyOrField<T>(T value);

        /// <summary>
        /// Registers a factory function to be invoked for properties and fields of type <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
        TReturn WithPropertyOrField<T>(Func<T> func);

        /// <summary>
        /// Registers a value to be used for properties and fields of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        /// <remarks>
        /// The same instance is provided on every resolution, for every fixture this registration applies to.
        /// If <typeparamref name="T"/> is a mutable reference type, configuring members of the provided instance changes it for every fixture that receives it.
        /// Use the <c>Func&lt;T&gt;</c> overload to provide a new instance per resolution.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        TReturn WithPropertyOrField<T>(T value, string name);

        /// <summary>
        /// Registers a factory function to be invoked for properties and fields of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
        TReturn WithPropertyOrField<T>(Func<T> func, string name);
    }
}
