namespace FixtureBuilder
{
    public static class Fixture
    {
        /// <summary>
        /// Creates a new instance of a fixture constructor for the specified entity type.
        /// </summary>
        /// <typeparam name="T">The type of the entity for which the fixture constructor is created. Must be a reference type.</typeparam>
        /// <returns>An instance of <see cref="IFixtureConstructor{T}"/> for the specified entity type.</returns>
        public static IFixtureConstructor<T> New<T>() where T : class => new Fixture<T>();

        /// <summary>
        /// Creates a new fixture configurator for the specified entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity to configure. Must be a reference type.</typeparam>
        /// <param name="instance">The entity instance to be used for configuration. Cannot be <see langword="null"/>.</param>
        /// <returns>An <see cref="IFixtureConfigurator{T}"/> instance for configuring the specified entity.</returns>
        public static IFixtureConfigurator<T> New<T>(T instance) where T : class => new Fixture<T>(instance);
    }
}
