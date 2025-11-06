using System.Linq.Expressions;

namespace Shared.TestUtilities.Fixtures
{
	public interface IFixtureConstructor<TEntity> : IFixtureConfigurator<TEntity> where TEntity : class
	{
		IFixtureConfigurator<TEntity> BypassConstructor();
		IFixtureConfigurator<TEntity> UseConstructor(params object[] args);
	}

	public interface IFixtureConfigurator<TEntity> where TEntity : class
	{
		IFixtureConfigurator<TTarget> CastTo<TTarget>() where TTarget : class;
		IFixtureConfigurator<TEntity> With<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value);
		IFixtureConfigurator<TEntity> With<TInterface, TProp>(Expression<Func<TInterface, TProp>> expr, TProp value);
		IFixtureConfigurator<TEntity> WithField<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value);
		IFixtureConfigurator<TEntity> WithField<TInterface, TProp>(Expression<Func<TInterface, TProp>> expr, TProp value);
		IFixtureConfigurator<TEntity> WithField(string fieldName, object value);
		IFixtureConfigurator<TEntity> WithField(string fieldName, IEnumerable<object> value);
		IFixtureConfigurator<TEntity> WithField<TProp>(string fieldName, Expression<Func<TEntity, TProp>> expr, TProp value);
		IFixtureConfigurator<TEntity> WithField<TInterface, TProp>(string fieldName, Expression<Func<TInterface, TProp>> expr, TProp value);
		IFixtureConfigurator<TEntity> WithSetter<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value);
		IFixtureConfigurator<TEntity> WithSetter<TInterface, TProp>(Expression<Func<TInterface, TProp>> expr, TProp value);
		TEntity Build();
	}
}
