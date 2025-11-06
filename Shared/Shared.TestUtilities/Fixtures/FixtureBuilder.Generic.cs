using System.Linq.Expressions;

namespace Shared.TestUtilities.Fixtures
{
	internal class FixtureBuilder<TEntity> : IFixtureConstructor<TEntity>, IFixtureConfigurator<TEntity> where TEntity : class
	{
		private TEntity _fixture = null!;

		TEntity IFixtureConfigurator<TEntity>.Build()
		{
			_fixture ??= (TEntity)InstantiationHelpers.GetInstantiatedInstance(typeof(TEntity), instantiateMembers: true);

			return _fixture;
		}

		internal FixtureBuilder()
		{
			if (typeof(TEntity).IsInterface)
				throw new InvalidOperationException($"Cannot instantiate interface type: {typeof(TEntity).Name}");
		}

		internal FixtureBuilder(TEntity entity) => _fixture = entity;

		IFixtureConfigurator<TEntity> IFixtureConstructor<TEntity>.BypassConstructor()
		{
			var instance = InstantiationHelpers.BypassConstructor(typeof(TEntity))
				?? throw new InvalidOperationException($"Failed to instantiate {typeof(TEntity)}");

			_fixture = (TEntity)instance;
			InstantiationHelpers.InstantiateMembers(_fixture);

			return this;
		}

		IFixtureConfigurator<TEntity> IFixtureConstructor<TEntity>.UseConstructor(params object[] args)
		{
			var instance = InstantiationHelpers.UseConstructor(typeof(TEntity), args)
				?? throw new MissingMethodException($"Failed to instantiate {typeof(TEntity)}");

			_fixture = (TEntity)instance;
			InstantiationHelpers.InstantiateMembers(_fixture);

			return this;
		}

		IFixtureConfigurator<TTarget> IFixtureConfigurator<TEntity>.CastTo<TTarget>()
		{
			_fixture ??= (TEntity)InstantiationHelpers.GetInstantiatedInstance(typeof(TEntity), instantiateMembers: true);

			if (_fixture is not TTarget target)
				throw new InvalidCastException($"Cannot cast {typeof(TEntity).Name} to {typeof(TTarget).Name}");

			return new FixtureBuilder<TTarget>(target);
		}

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithField(string fieldName, object value)
		{
			_fixture ??= (TEntity)InstantiationHelpers.GetInstantiatedInstance(typeof(TEntity), instantiateMembers: true);

			FieldHelpers.SetField(_fixture, fieldName, value, allowNonCollection: true);

			return this;
		}

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithField(string fieldName, IEnumerable<object> values)
		{
			_fixture ??= (TEntity)InstantiationHelpers.GetInstantiatedInstance(typeof(TEntity), instantiateMembers: true);

			FieldHelpers.SetField(_fixture, fieldName, values, allowNonCollection: false);

			return this;
		}

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithField<TProp>(
			Expression<Func<TEntity, TProp>> expr,
			TProp value)
			=> WithFieldInternal(expr, value);

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithField<TProp>(string fieldName, Expression<Func<TEntity, TProp>> expr, TProp value)
			=> WithFieldInternal(expr, value, fieldName);

		private FixtureBuilder<TEntity> WithFieldInternal<TProp>(
			Expression<Func<TEntity, TProp>> expr,
			TProp value,
			string? fieldName = null)
		{
			_fixture ??= (TEntity)InstantiationHelpers.GetInstantiatedInstance(typeof(TEntity), instantiateMembers: true);

			var (instance, property) = ExpressionHelpers.ResolvePropertyPath(_fixture, expr);

			var fieldNames = fieldName == null ? FieldHelpers.GetCommonFieldNames(property.Name) : [fieldName];
			var declaringType = property.DeclaringType;

			if (declaringType != null && declaringType.IsInterface)
			{
				var explicitFieldName = $"<{declaringType.FullName}.{property.Name}>k__BackingField";
				fieldNames = [explicitFieldName, .. fieldNames];
			}

			if (FieldHelpers.TryGetField(_fixture.GetType(), fieldNames, out var backingField)) { }
			else if (declaringType != null && FieldHelpers.TryGetField(declaringType, fieldNames, out backingField)) { }
			else throw new InvalidOperationException($"Backing field not found for property {property.Name}");

			backingField.SetValue(instance, value);
			return this;
		}

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithSetter<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value)
		{
			if (!ExpressionHelpers.IsPropertyWritable(expr)) throw new InvalidOperationException($"{typeof(TProp).Name} Does not contain a setter");
			return WithSetterInternal(expr, value);
		}

		private FixtureBuilder<TEntity> WithSetterInternal<TProp>(
			Expression<Func<TEntity, TProp>> expr,
			TProp value)
		{
			_fixture ??= (TEntity)InstantiationHelpers.GetInstantiatedInstance(typeof(TEntity), instantiateMembers: true);

			var (instance, property) = ExpressionHelpers.ResolvePropertyPath(_fixture, expr);

			if (property != null) property.SetValue(instance, value);
			else throw new InvalidOperationException("Unsupported member type");

			return this;
		}

		IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.With<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value)
		{
			return WithInternal(expr, value);
		}

		private FixtureBuilder<TEntity> WithInternal<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value)
		{
			_fixture ??= (TEntity)InstantiationHelpers.GetInstantiatedInstance(typeof(TEntity), instantiateMembers: true);

			if (ExpressionHelpers.IsPropertyWritable(expr)) return WithSetterInternal(expr, value);
			else return WithFieldInternal(expr, value);
		}
	}
}
