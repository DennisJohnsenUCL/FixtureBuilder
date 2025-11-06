using System.Linq.Expressions;

namespace Shared.TestUtilities.Fixtures
{
	public static class FixtureBuilderExtensions
	{
		public static TEntity With<TEntity, TProp>(this TEntity fixture, Expression<Func<TEntity, TProp>> expr, TProp value) where TEntity : class
		{
			return FixtureBuilder.New(fixture).With<TProp>(expr, value).Build();
		}

		public static TEntity With<TEntity, TInterface, TProp>(this TEntity fixture, Expression<Func<TInterface, TProp>> expr, TProp value) where TEntity : class
		{
			return FixtureBuilder.New(fixture).With(expr, value).Build();
		}

		public static TEntity WithField<TEntity, TProp>(this TEntity fixture, Expression<Func<TEntity, TProp>> expr, TProp value) where TEntity : class
		{
			return FixtureBuilder.New(fixture).WithField<TProp>(expr, value).Build();
		}

		public static TEntity WithField<TEntity, TInterface, TProp>(this TEntity fixture, Expression<Func<TInterface, TProp>> expr, TProp value) where TEntity : class
		{
			return FixtureBuilder.New(fixture).WithField(expr, value).Build();
		}

		public static TEntity WithField<TEntity>(this TEntity fixture, string fieldName, object value) where TEntity : class
		{
			return FixtureBuilder.New(fixture).WithField(fieldName, value).Build();
		}

		public static TEntity WithField<TEntity>(this TEntity fixture, string fieldName, IEnumerable<object> value) where TEntity : class
		{
			return FixtureBuilder.New(fixture).WithField(fieldName, value).Build();
		}

		public static TEntity WithField<TEntity, TProp>(this TEntity fixture, string fieldName, Expression<Func<TEntity, TProp>> expr, TProp value) where TEntity : class
		{
			return FixtureBuilder.New(fixture).WithField<TProp>(fieldName, expr, value).Build();
		}

		public static TEntity WithField<TEntity, TInterface, TProp>(this TEntity fixture, string fieldName, Expression<Func<TInterface, TProp>> expr, TProp value) where TEntity : class
		{
			return FixtureBuilder.New(fixture).WithField(fieldName, expr, value).Build();
		}

		public static TEntity WithSetter<TEntity, TProp>(this TEntity fixture, Expression<Func<TEntity, TProp>> expr, TProp value) where TEntity : class
		{
			return FixtureBuilder.New(fixture).WithSetter<TProp>(expr, value).Build();
		}

		public static TEntity WithSetter<TEntity, TInterface, TProp>(this TEntity fixture, Expression<Func<TInterface, TProp>> expr, TProp value) where TEntity : class
		{
			return FixtureBuilder.New(fixture).WithSetter(expr, value).Build();
		}
	}
}
