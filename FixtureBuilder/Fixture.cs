namespace FixtureBuilder
{
    /// <summary>
    /// Static entry point for creating and configuring individual fixtures. Use <see cref="New{T}()"/> to start
    /// building a fixture from scratch, or <see cref="New{T}(T)"/> to configure an existing instance.
    /// For shared configuration across multiple fixtures, use <see cref="FixtureFactory"/> instead.
    /// </summary>
    public static class Fixture
    {
        /// <summary>
        /// Creates a new instance of a fixture constructor for the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object for which the fixture constructor is created. Must be a reference type.</typeparam>
        /// <returns>An instance of <see cref="IFixtureConstructor{T}"/> for the specified type.</returns>
        public static IFixtureConstructor<T> New<T>() where T : class => new Fixture<T>();

        /// <summary>
        /// Creates a new fixture configurator for the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the object to configure. Must be a reference type.</typeparam>
        /// <param name="instance">The instance to be used for configuration. Cannot be <see langword="null"/>.</param>
        /// <returns>An <see cref="IFixtureConfigurator{T}"/> instance for configuring the specified object.</returns>
        public static IFixtureConfigurator<T> New<T>(T instance) where T : class => new Fixture<T>(instance);
    }
}
