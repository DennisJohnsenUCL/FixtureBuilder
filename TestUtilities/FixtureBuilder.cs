using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TestUtilities
{
	internal class FixtureBuilder<TEntity> : IFixtureConstructor<TEntity>, IFixtureConfigurator<TEntity> where TEntity : class
	{
		private TEntity _fixture = null!;

		TEntity IFixtureConfigurator<TEntity>.Build()
		{
			_fixture ??= (TEntity)GetInstantiatedInstance(typeof(TEntity));

			return _fixture;
		}

		internal FixtureBuilder() { }

		internal FixtureBuilder(TEntity entity) => _fixture = entity;

		IFixtureConfigurator<TEntity> IFixtureConstructor<TEntity>.BypassConstructor()
		{
			var instance = BypassConstructorInternal(typeof(TEntity))
				?? throw new InvalidOperationException($"Failed to instantiate {typeof(TEntity)}");

			_fixture = (TEntity)instance;
			return this;
		}

		private static object? BypassConstructorInternal(Type type)
		{
			try { return RuntimeHelpers.GetUninitializedObject(type); }
			catch { return null; }
		}

		IFixtureConfigurator<TEntity> IFixtureConstructor<TEntity>.UseConstructor(params object[] args)
		{
			var instance = UseConstructorInternal(typeof(TEntity), args)
				?? throw new MissingMethodException($"Failed to instantiate {typeof(TEntity)}");

			_fixture = (TEntity)instance;
			return this;
		}

		private static object? UseConstructorInternal(Type type, params object[] args)
		{
			try
			{
				return Activator.CreateInstance(
					type,
					BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
					binder: null,
					args: args,
					culture: CultureInfo.CurrentCulture);
			}
			catch { return null; }
		}

		private static object GetInstantiatedInstance(Type type)
		{
			var instance = UseConstructorInternal(type) ?? BypassConstructorInternal(type);
			return instance ?? throw new InvalidOperationException($"Failed to instantiate {type}");
		}

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithField<TProp>(
			Expression<Func<TEntity, TProp>> expr,
			TProp value)
			=> WithFieldInternal(expr, value);

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithField<TInterface, TProp>(
			Expression<Func<TInterface, TProp>> expr,
			TProp value)
		{
			ValidateInterface(typeof(TInterface));

			var lambda = ConvertExpression(expr);
			return WithFieldInternal(lambda, value);
		}

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithField(string fieldName, object value)
		{
			_fixture ??= (TEntity)GetInstantiatedInstance(typeof(TEntity));

			if (!TryGetField(typeof(TEntity), fieldName, out var fieldInfo))
				throw new InvalidOperationException($"Field '{fieldName}' not found.");

			fieldInfo.SetValue(_fixture, value);

			return this;
		}

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithField<TProp>(string fieldName, Expression<Func<TEntity, TProp>> expr, TProp value)
			=> WithFieldInternal(expr, value, fieldName);

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithField<TInterface, TProp>(string fieldName, Expression<Func<TInterface, TProp>> expr, TProp value)
		{
			ValidateInterface(typeof(TInterface));

			var lambda = ConvertExpression(expr);

			return WithFieldInternal(lambda, value, fieldName);
		}

		private FixtureBuilder<TEntity> WithFieldInternal<TProp>(
			Expression<Func<TEntity, TProp>> expr,
			TProp value,
			string? fieldName = null)
		{
			_fixture ??= (TEntity)GetInstantiatedInstance(typeof(TEntity));

			var (instance, property) = ResolvePropertyPath(_fixture, expr);

			var fieldNames = fieldName == null ? GetFieldNames(property.Name) : [fieldName];

			if (property.DeclaringType != null && property.DeclaringType.IsInterface)
			{
				var explicitFieldName = $"<{property.DeclaringType.FullName}.{property.Name}>k__BackingField";
				fieldNames = [explicitFieldName, .. fieldNames];
			}

			if (TryGetFixtureField(fieldNames, out var backingField)) { }
			else if (TryGetDeclaredField(property, fieldNames, out backingField)) { }
			else throw new InvalidOperationException($"Backing field not found for property {property.Name}");

			backingField.SetValue(instance, value);
			return this;
		}

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithSetter<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value)
		{
			if (!IsPropertyWritable(expr)) throw new InvalidOperationException($"{typeof(TProp).Name} Does not contain a setter");
			return WithSetterInternal(expr, value);
		}

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithSetter<TInterface, TProp>(
			Expression<Func<TInterface, TProp>> expr,
			TProp value)
		{
			ValidateInterface(typeof(TInterface));
			if (!IsPropertyWritable(expr)) throw new InvalidOperationException($"{typeof(TProp).Name} Does not contain a setter");

			var lambda = ConvertExpression(expr);

			return WithSetterInternal(lambda, value);
		}

		private FixtureBuilder<TEntity> WithSetterInternal<TProp>(
			Expression<Func<TEntity, TProp>> expr,
			TProp value)
		{
			_fixture ??= (TEntity)GetInstantiatedInstance(typeof(TEntity));

			var (instance, property) = ResolvePropertyPath(_fixture, expr);

			if (property != null) property.SetValue(instance, value);
			else throw new InvalidOperationException("Unsupported member type");

			return this;
		}

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.With<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value)
		{
			_fixture ??= (TEntity)GetInstantiatedInstance(typeof(TEntity));

			return WithInternal(expr, value);
		}

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.With<TInterface, TProp>(Expression<Func<TInterface, TProp>> expr, TProp value)
		{
			ValidateInterface(typeof(TInterface));

			var lambda = ConvertExpression(expr);

			return WithInternal(lambda, value);
		}

		private FixtureBuilder<TEntity> WithInternal<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value)
		{
			_fixture ??= (TEntity)GetInstantiatedInstance(typeof(TEntity));

			if (IsPropertyWritable(expr)) return WithSetterInternal(expr, value);
			else return WithFieldInternal(expr, value);
		}

		private static Expression<Func<TEntity, TProp>> ConvertExpression<TInterface, TProp>(
			Expression<Func<TInterface, TProp>> expr)
		{
			if (expr.Body is not MemberExpression memberExpr)
				throw new ArgumentException("Expression body must be a member expression", nameof(expr));

			var param = Expression.Parameter(typeof(TEntity), expr.Parameters[0].Name);

			Expression Rewrite(MemberExpression me)
			{
				if (me.Expression is ParameterExpression pe && pe == expr.Parameters[0])
				{
					var converted = Expression.Convert(param, typeof(TInterface));
					return Expression.MakeMemberAccess(converted, me.Member);
				}
				else if (me.Expression is MemberExpression inner)
				{
					var innerExpr = Rewrite(inner);
					return Expression.MakeMemberAccess(innerExpr, me.Member);
				}
				else
				{
					throw new InvalidOperationException($"Unexpected expression node: {me.Expression?.NodeType}");
				}
			}

			var newBody = Rewrite(memberExpr);
			return Expression.Lambda<Func<TEntity, TProp>>(newBody, param);
		}

		private static bool IsPropertyWritable<TTarget, TProp>(Expression<Func<TTarget, TProp>> expr)
		{
			if (expr.Body is not MemberExpression memberExpr)
				throw new ArgumentException("Expression must be a property access", nameof(expr));

			if (memberExpr.Member is not PropertyInfo propInfo)
				throw new ArgumentException("Expression must be a property", nameof(expr));

			return propInfo.CanWrite;
		}

		private static (object instance, PropertyInfo property) ResolvePropertyPath<TProp>(TEntity root, Expression<Func<TEntity, TProp>> expr)
		{
			var memberExpr = expr.Body as MemberExpression
				?? throw new ArgumentException("Expression must be a property access", nameof(expr));

			var members = new Stack<MemberInfo>();
			while (memberExpr != null)
			{
				members.Push(memberExpr.Member);
				memberExpr = memberExpr.Expression as MemberExpression;
			}

			object current = root;
			MemberInfo currentMember;

			while (members.Count > 1)
			{
				currentMember = members.Pop();
				var prop = (PropertyInfo)currentMember;

				var parent = current;
				current = prop.GetValue(parent)!;

				if (current == null)
				{
					var type = prop.PropertyType;
					current = GetInstantiatedInstance(type);
					prop.SetValue(parent, current);
				}
			}

			currentMember = members.Pop();
			var finalProp = (PropertyInfo)currentMember;

			return (current, finalProp);
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

		private static void ValidateInterface(Type TInterface)
		{
			if (!TInterface.IsInterface) throw new ArgumentException($"{TInterface} must be an interface type");
			if (!typeof(TEntity).IsAssignableTo(TInterface)) throw new ArgumentException($"{TInterface} must be assignable from TEntity");
		}

		private static string[] GetFieldNames(string propName) =>
			[$"<{propName}>k__BackingField",
			$"_{char.ToLower(propName[0]) + propName[1..]}",
			$"{char.ToLower(propName[0]) + propName[1..]}"];
	}

	public static class FixtureBuilder
	{
		public static IFixtureConstructor<TEntity> New<TEntity>() where TEntity : class => new FixtureBuilder<TEntity>();
		public static IFixtureConfigurator<TEntity> New<TEntity>(TEntity entity) where TEntity : class => new FixtureBuilder<TEntity>(entity);
	}

	public interface IFixtureConstructor<TEntity> : IFixtureConfigurator<TEntity> where TEntity : class
	{
		IFixtureConfigurator<TEntity> BypassConstructor();
		IFixtureConfigurator<TEntity> UseConstructor(params object[] args);
	}

	public interface IFixtureConfigurator<TEntity> where TEntity : class
	{
		IFixtureConfigurator<TEntity> With<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value);
		IFixtureConfigurator<TEntity> With<TInterface, TProp>(Expression<Func<TInterface, TProp>> expr, TProp value);
		IFixtureConfigurator<TEntity> WithField<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value);
		IFixtureConfigurator<TEntity> WithField<TInterface, TProp>(Expression<Func<TInterface, TProp>> expr, TProp value);
		IFixtureConfigurator<TEntity> WithField(string fieldName, object value);
		IFixtureConfigurator<TEntity> WithField<TProp>(string fieldName, Expression<Func<TEntity, TProp>> expr, TProp value);
		IFixtureConfigurator<TEntity> WithField<TInterface, TProp>(string fieldName, Expression<Func<TInterface, TProp>> expr, TProp value);
		IFixtureConfigurator<TEntity> WithSetter<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value);
		IFixtureConfigurator<TEntity> WithSetter<TInterface, TProp>(Expression<Func<TInterface, TProp>> expr, TProp value);
		TEntity Build();
	}
}
