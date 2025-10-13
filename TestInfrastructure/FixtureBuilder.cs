using System.Diagnostics.CodeAnalysis;
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

			var fieldNames = GetFieldNames(propInfo.Name);

			return With(propInfo, value, fieldNames);
		}

		public FixtureBuilder<TEntity> With<TInterface, TProp>(Expression<Func<TInterface, TProp>> expr, TProp value)
		{
			if (!typeof(TInterface).IsInterface) throw new ArgumentException($"{typeof(TInterface)} must be an interface type");
			if (!typeof(TEntity).IsAssignableTo(typeof(TInterface))) throw new ArgumentException($"{typeof(TInterface)} must be assignable from TEntity");

			var lambda = ConvertExpression(expr);

			var propInfo = GetPropertyInfo(lambda);

			var fieldNames = new[] { $"<{typeof(TInterface).FullName}.{propInfo.Name}>k__BackingField" }.Concat(GetFieldNames(propInfo.Name)).ToArray();

			return With(propInfo, value, fieldNames);
		}

		private FixtureBuilder<TEntity> With<TProp>(PropertyInfo propInfo, TProp value, string[] fieldNames)
		{
			if (TryGetFixtureField(fieldNames, out FieldInfo backingField)) { }
			else if (TryGetDeclaredField(propInfo, fieldNames, out backingField)) { }
			else { throw new InvalidOperationException($"Backing field not found for property {propInfo.Name}"); }
			backingField.SetValue(_fixture, value);
			return this;
		}

		public FixtureBuilder<TEntity> WithField(string fieldName, object value)
		{
			if (!TryGetField(fieldName, out var fieldInfo))
				throw new InvalidOperationException($"Field '{fieldName}' not found.");

			fieldInfo.SetValue(_fixture, value);

			return this;
		}

		public FixtureBuilder<TEntity> WithSetter<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value)
		{
			var propInfo = GetPropertyInfo(expr);

			var target = Expression.Parameter(typeof(TEntity), "target");
			var val = Expression.Parameter(typeof(TProp), "value");
			var setExpr = Expression.Assign(Expression.Property(target, propInfo), val);
			var setter = Expression.Lambda<Action<TEntity, TProp>>(setExpr, target, val).Compile();

			setter(_fixture, value);

			return this;
		}

		private bool TryGetFixtureField(string[] fieldNames, [NotNullWhen(true)] out FieldInfo fieldInfo)
		{
			var fixtureType = _fixture.GetType();

			TryGetField(fixtureType, fieldNames, out fieldInfo);
			return fieldInfo != null;
		}

		private static bool TryGetDeclaredField(PropertyInfo propInfo, string[] fieldNames, [NotNullWhen(true)] out FieldInfo fieldInfo)
		{
			var declaringType = propInfo.DeclaringType
				?? throw new InvalidOperationException($"Property {propInfo.Name} has no declaring type");

			TryGetField(declaringType, fieldNames, out fieldInfo);
			return fieldInfo != null;
		}

		private bool TryGetField(string fieldName, [NotNullWhen(true)] out FieldInfo fieldInfo) => TryGetField(_fixture.GetType(), fieldName, out fieldInfo);

		private static bool TryGetField(Type type, string fieldName, [NotNullWhen(true)] out FieldInfo fieldInfo)
		{
			fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;
			return fieldInfo != null;
		}

		private static bool TryGetField(Type type, string[] fieldNames, [NotNullWhen(true)] out FieldInfo fieldInfo)
		{
			foreach (var name in fieldNames)
			{
				if (TryGetField(type, name, out fieldInfo)) return true;
			}
			fieldInfo = null!;
			return false;
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

		private static Expression<Func<TEntity, TProp>> ConvertExpression<TInterface, TProp>(Expression<Func<TInterface, TProp>> expr)
		{
			var param = Expression.Parameter(typeof(TEntity), expr.Parameters[0].Name);
			var body = Expression.PropertyOrField(Expression.Convert(param, typeof(TInterface)),
				((MemberExpression)expr.Body).Member.Name);
			var lambda = Expression.Lambda<Func<TEntity, TProp>>(body, param);
			return lambda;
		}

		private static string[] GetFieldNames(string propName) =>
			[$"<{propName}>k__BackingField",
			$"_{char.ToLower(propName[0]) + propName[1..]}",
			$"{char.ToLower(propName[0]) + propName[1..]}"];
	}

	public static class FixtureBuilder
	{
		public static FixtureBuilder<TEntity> New<TEntity>() where TEntity : class => new();
		public static FixtureBuilder<TEntity> New<TEntity>(TEntity entity) where TEntity : class => new(entity);
	}
}
