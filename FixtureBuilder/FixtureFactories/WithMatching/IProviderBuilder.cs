namespace FixtureBuilder.FixtureFactories.WithMatching
{
    public interface IProviderBuilder<TReturn>
    {
        /// <summary>
        /// Registers a value to be used for all members and parameters of type <typeparamref name="T"/>.
        /// </summary>
        TReturn With<T>(T value);

        /// <summary>
        /// Registers a factory function to be invoked for all members and parameters of type <typeparamref name="T"/>.
        /// </summary>
        TReturn With<T>(Func<T> func);

        /// <summary>
        /// Registers a value to be used for members and parameters of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        TReturn With<T>(T value, string name);

        /// <summary>
        /// Registers a factory function to be invoked for members and parameters of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        TReturn With<T>(Func<T> func, string name);

        /// <summary>
        /// Registers a value to be used for constructor parameters of type <typeparamref name="T"/>.
        /// </summary>
        TReturn WithParameter<T>(T value);

        /// <summary>
        /// Registers a factory function to be invoked for constructor parameters of type <typeparamref name="T"/>.
        /// </summary>
        TReturn WithParameter<T>(Func<T> func);

        /// <summary>
        /// Registers a value to be used for constructor parameters of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        TReturn WithParameter<T>(T value, string name);

        /// <summary>
        /// Registers a factory function to be invoked for constructor parameters of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        TReturn WithParameter<T>(Func<T> func, string name);

        /// <summary>
        /// Registers a value to be used for properties and fields of type <typeparamref name="T"/>.
        /// </summary>
        TReturn WithPropertyOrField<T>(T value);

        /// <summary>
        /// Registers a factory function to be invoked for properties and fields of type <typeparamref name="T"/>.
        /// </summary>
        TReturn WithPropertyOrField<T>(Func<T> func);

        /// <summary>
        /// Registers a value to be used for properties and fields of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        TReturn WithPropertyOrField<T>(T value, string name);

        /// <summary>
        /// Registers a factory function to be invoked for properties and fields of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        TReturn WithPropertyOrField<T>(Func<T> func, string name);
    }
}
