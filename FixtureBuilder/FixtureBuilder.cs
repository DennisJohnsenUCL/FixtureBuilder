using Shared.TestUtilities.Fixtures;

namespace FixtureBuilder
{
    public static class FixtureBuilder
    {
        /// <summary>
        /// Creates a new instance of a fixture constructor for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity for which the fixture constructor is created. Must be a reference type.</typeparam>
        /// <returns>An instance of <see cref="IFixtureConstructor{TEntity}"/> for the specified entity type.</returns>
        public static IFixtureConstructor<TEntity> New<TEntity>() where TEntity : class => new FixtureBuilder<TEntity>();
        /// <summary>
        /// Creates a new fixture configurator for the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to configure. Must be a reference type.</typeparam>
        /// <param name="entity">The entity instance to be used for configuration. Cannot be <see langword="null"/>.</param>
        /// <returns>An <see cref="IFixtureConfigurator{TEntity}"/> instance for configuring the specified entity.</returns>
        public static IFixtureConfigurator<TEntity> New<TEntity>(TEntity entity) where TEntity : class => new FixtureBuilder<TEntity>(entity);
    }
}
