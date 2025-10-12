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

			if (TryGetFixtureField(propInfo.Name, out FieldInfo backingField)) { }
			else if (TryGetDeclaredField(propInfo, out backingField)) { }
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

		private bool TryGetFixtureField(string propName, [NotNullWhen(true)] out FieldInfo fieldInfo)
		{
			var fieldNames = GetFieldNames(propName);

			if (TryGetField(fieldNames[0], out fieldInfo)) { }
			else if (TryGetField(fieldNames[1], out fieldInfo)) { }
			else if (TryGetField(fieldNames[2], out fieldInfo)) { }
			return fieldInfo != null;
		}

		private static bool TryGetDeclaredField(PropertyInfo propInfo, [NotNullWhen(true)] out FieldInfo fieldInfo)
		{
			var fieldNames = GetFieldNames(propInfo.Name);

			var declaringType = propInfo.DeclaringType
				?? throw new InvalidOperationException($"Property {propInfo.Name} has no declaring type");

			if (TryGetField(declaringType, fieldNames[0], out fieldInfo)) { }
			else if (TryGetField(declaringType, fieldNames[1], out fieldInfo)) { }
			else if (TryGetField(declaringType, fieldNames[2], out fieldInfo)) { }
			return fieldInfo != null;
		}

		private bool TryGetField(string fieldName, [NotNullWhen(true)] out FieldInfo fieldInfo) => TryGetField(_fixture.GetType(), fieldName, out fieldInfo);

		private static bool TryGetField(Type type, string fieldName, [NotNullWhen(true)] out FieldInfo fieldInfo)
		{
			fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;
			return fieldInfo != null;
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
