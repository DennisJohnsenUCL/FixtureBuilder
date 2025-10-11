using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TestUtilities
{
	public class FixtureBuilder<TEntity> where TEntity : class
	{
		private readonly TEntity _fixture;
		public TEntity Build() => _fixture;

		internal FixtureBuilder()
		{
			_fixture = (TEntity)RuntimeHelpers.GetUninitializedObject(typeof(TEntity));
		}

		internal FixtureBuilder(TEntity entity) => _fixture = entity;

		public FixtureBuilder<TEntity> With<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value)
		{
			var propInfo = GetPropertyInfo(expr);

			var backingField = _fixture.GetType().GetField($"<{propInfo.Name}>k__BackingField",
				BindingFlags.Instance | BindingFlags.NonPublic)
				?? throw new InvalidOperationException($"Backing field not found for property {propInfo.Name}");

			backingField.SetValue(_fixture, value);

			return this;
		}

		public FixtureBuilder<TEntity> WithFieldUnsafe(string fieldName, object value)
		{
			var fieldInfo = _fixture.GetType().GetField(fieldName,
				BindingFlags.Instance | BindingFlags.NonPublic)
				?? throw new InvalidOperationException($"Field '{fieldName}' not found.");

			fieldInfo.SetValue(_fixture, value);

			return this;
		}

		private static PropertyInfo GetPropertyInfo<TProp>(Expression<Func<TEntity, TProp>> expr)
		{
			if (expr.Body is MemberExpression memberExpr && memberExpr.Member is PropertyInfo propInfo)
			{
				return propInfo;
			}

			if (expr.Body is UnaryExpression unary && unary.Operand is MemberExpression innerMember
				&& innerMember.Member is PropertyInfo innerProp)
			{
				return innerProp;
			}

			throw new ArgumentException("Expression must be a property access", nameof(expr));
		}
	}

	public static class FixtureBuilder
	{
		public static FixtureBuilder<TEntity> New<TEntity>() where TEntity : class => new();
		public static FixtureBuilder<TEntity> New<TEntity>(TEntity entity) where TEntity : class => new(entity);
	}
}
