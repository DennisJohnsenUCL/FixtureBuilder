namespace Shared.TestUtilities.Fixtures
{
	public static class FixtureBuilder
	{
		public static IFixtureConstructor<TEntity> New<TEntity>() where TEntity : class => new FixtureBuilder<TEntity>();
		public static IFixtureConfigurator<TEntity> New<TEntity>(TEntity entity) where TEntity : class => new FixtureBuilder<TEntity>(entity);
	}
}
