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
        TReturn With<T>(T value);

        /// <summary>
        /// Registers a factory function to be invoked for all members and parameters of type <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
        TReturn With<T>(Func<T> func);

        /// <summary>
        /// Registers a value to be used for members and parameters of type <typeparamref name="T"/> with a matching name.
        /// </summary>
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
        TReturn WithParameter<T>(T value);

        /// <summary>
        /// Registers a factory function to be invoked for constructor parameters of type <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
        TReturn WithParameter<T>(Func<T> func);

        /// <summary>
        /// Registers a value to be used for constructor parameters of type <typeparamref name="T"/> with a matching name.
        /// </summary>
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
        TReturn WithPropertyOrField<T>(T value);

        /// <summary>
        /// Registers a factory function to be invoked for properties and fields of type <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
        TReturn WithPropertyOrField<T>(Func<T> func);

        /// <summary>
        /// Registers a value to be used for properties and fields of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        TReturn WithPropertyOrField<T>(T value, string name);

        /// <summary>
        /// Registers a factory function to be invoked for properties and fields of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> or <paramref name="name"/> is <see langword="null"/>.</exception>
        TReturn WithPropertyOrField<T>(Func<T> func, string name);
    }
}
