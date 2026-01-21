using FixtureBuilder.Helpers;
using System.Collections;
using System.Linq.Expressions;

namespace FixtureBuilder
{
    internal class Fixture<TEntity> : IFixtureConstructor<TEntity>, IFixtureConfigurator<TEntity> where TEntity : class
    {
        private TEntity _fixture = null!;

        /// <summary>
        /// Builds and returns the configured fixture instance of the specified type.
        /// </summary>
        /// <remarks>If the fixture instance has not been previously created, a new instance is instantiated  with its
        /// members initialized. Subsequent calls will return the same instance.</remarks>
        /// <returns>The configured fixture instance of type <typeparamref name="TEntity"/>.</returns>
        TEntity IFixtureConfigurator<TEntity>.Build()
        {
            _fixture ??= (TEntity)InstantiationHelpers.GetInstantiatedInstance(typeof(TEntity), instantiateMembers: true);

            return _fixture;
        }

        internal Fixture()
        {
            if (typeof(TEntity).IsInterface)
                throw new InvalidOperationException($"Cannot create fixtures of interface types: {typeof(TEntity).Name}. Please use concrete types for fixtures.");

            if (typeof(TEntity).IsAbstract)
                throw new InvalidOperationException($"Cannot create fixtures of abstract types: {typeof(TEntity).Name}. Please use concrete types for fixtures.");
        }

        internal Fixture(TEntity entity)
        {
            _fixture = entity ?? throw new InvalidOperationException($"Cannot use a null instance as fixture {typeof(TEntity).Name}. Please use generic parameter instead for generating new fixtures.");
        }

        /// <summary>
        /// Creates an instance of the entity type <typeparamref name="TEntity"/> without invoking its constructor.
        /// </summary>
        /// <remarks>This method bypasses the constructor of <typeparamref name="TEntity"/> to create an uninitialized
        /// instance. After instantiation, the members of the instance are initialized using default values.</remarks>
        /// <returns>An <see cref="IFixtureConfigurator{TEntity}"/> instance for further configuration of the created entity.</returns>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<TEntity> IFixtureConstructor<TEntity>.BypassConstructor()
        {
            var instance = InstantiationHelpers.BypassConstructor(typeof(TEntity))
                ?? throw new InvalidOperationException($"Failed to instantiate {typeof(TEntity)} by bypassing constructor. Please try to instantiate with 'UseConstructor' instead.");

            _fixture = (TEntity)instance;
            InstantiationHelpers.InstantiateMembers(_fixture);

            return this;
        }

        /// <summary>
        /// Configures the fixture to use a specific constructor for creating an instance of the entity.
        /// </summary>
        /// <param name="args">The arguments to pass to the constructor. The arguments must match the constructor's parameter types and order.</param>
        /// <returns>An <see cref="IFixtureConfigurator{TEntity}"/> instance for further configuration.</returns>
        /// <exception cref="MissingMethodException"/>
        IFixtureConfigurator<TEntity> IFixtureConstructor<TEntity>.UseConstructor(params object[] args)
        {
            var instance = InstantiationHelpers.UseConstructor(typeof(TEntity), args)
                ?? throw new MissingMethodException($"Failed to instantiate {typeof(TEntity)} with given constructor arguments. Please ensure a matching constructor exists.");

            _fixture = (TEntity)instance;
            InstantiationHelpers.InstantiateMembers(_fixture);

            return this;
        }

        /// <summary>
        /// Casts the current fixture to the specified target type.
        /// </summary>
        /// <typeparam name="TTarget">The type to which the fixture should be cast.</typeparam>
        /// <returns>An <see cref="IFixtureConfigurator{TTarget}"/> instance for the casted fixture.</returns>
        /// <exception cref="InvalidCastException"/>
        IFixtureConfigurator<TTarget> IFixtureConfigurator<TEntity>.CastTo<TTarget>()
        {
            _fixture ??= (TEntity)InstantiationHelpers.GetInstantiatedInstance(typeof(TEntity), instantiateMembers: true);

            if (_fixture is not TTarget target)
                throw new InvalidCastException($"Cannot cast {typeof(TEntity).Name} to {typeof(TTarget).Name}.");

            return new Fixture<TTarget>(target);
        }

        /// <summary>
        /// Configures the fixture by setting the specified field to the given value.
        /// </summary>
        /// <remarks>This method initializes the fixture instance if it has not already been created. The specified
        /// field is set directly, bypassing property setters. Use this method to configure fields that are not accessible
        /// through public properties.</remarks>
        /// <param name="fieldName">The name of the field to set. This must match the name of an existing field in the entity.</param>
        /// <param name="value">The value to assign to the specified field. The value must be compatible with the field's type.</param>
        /// <returns>The current <see cref="IFixtureConfigurator{TEntity}"/> instance, allowing for method chaining.</returns>
        IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithField<T>(string fieldName, T value)
        {
            _fixture ??= (TEntity)InstantiationHelpers.GetInstantiatedInstance(typeof(TEntity), instantiateMembers: true);

            if (!FieldHelpers.TryGetField(typeof(TEntity), fieldName, out var fieldInfo))
                throw new InvalidOperationException($"Field '{fieldName}' not found on {typeof(TEntity).Name}.");

            if (value == null && fieldInfo.FieldType.IsValueType && !(fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                throw new InvalidOperationException("Cannot assign null to a non-nullable value type. Consider passing default instead.");

            try { fieldInfo.SetValue(_fixture, value); }
            catch (Exception ex) { throw new InvalidOperationException($"Failed to assign {value} to field {fieldName}", ex); }

            return this;
        }

        /// <summary>
        /// Configures the fixture by setting the value of a field backing the specified property.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="expr">An expression that identifies the property whose backing field should be set.</param>
        /// <param name="value">The value to assign to the backing field.</param>
        /// <returns>The fixture configurator for method chaining.</returns>
        /// <remarks>
        /// The backing field name is automatically discovered using common naming conventions.
        /// Supports both regular properties and interface-implemented properties with explicit backing fields.
        /// </remarks>
        IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithField<TProp>(
            Expression<Func<TEntity, TProp>> expr,
            TProp value)
            => WithFieldInternal(expr, value);

        /// <summary>
        /// Configures the fixture by setting the value of a specifically named field backing the specified property.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="fieldName">The explicit name of the backing field to set.</param>
        /// <param name="expr">An expression that identifies the property whose backing field should be set.</param>
        /// <param name="value">The value to assign to the backing field.</param>
        /// <returns>The fixture configurator for method chaining.</returns>
        /// <remarks>
        /// Use this overload when you need to specify a non-standard backing field name that cannot be automatically discovered.
        /// </remarks>
        IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithField<TProp>(string fieldName, Expression<Func<TEntity, TProp>> expr, TProp value)
            => WithFieldInternal(expr, value, fieldName);

        private Fixture<TEntity> WithFieldInternal<TProp>(
            Expression<Func<TEntity, TProp>> expr,
            TProp value,
            string? fieldName = null)
        {
            _fixture ??= (TEntity)InstantiationHelpers.GetInstantiatedInstance(typeof(TEntity), instantiateMembers: true);

            var (instance, property) = ExpressionHelpers.ResolvePropertyPath(_fixture, expr);

            if (!FieldHelpers.TryGetPropertyBackingField<TEntity>(property, fieldName, out var backingField))
                throw new InvalidOperationException($"Backing field not found for property {property.Name}. Please specify the name of the backing field if not following standard naming.");

            var fieldType = backingField.FieldType;
            var sourceType = value?.GetType();

            //TODO: Check for IEnumerable and not assignable first, then try-catch setting it
            if (fieldType != sourceType
                && !fieldType.IsAssignableFrom(sourceType)
                && fieldType != typeof(string)
                && value != null
                && typeof(IEnumerable).IsAssignableFrom(fieldType)
                && typeof(IEnumerable).IsAssignableFrom(sourceType))
            {
                IEnumerable collection;
                if (CollectionHelpers.IsDictionary(fieldType)) collection = CollectionHelpers.CastToDictionary(fieldType, (IEnumerable)value);
                else collection = CollectionHelpers.CastToCollection(fieldType, (IEnumerable)value);
                backingField.SetValue(instance, collection);
            }
            else
            {
                backingField.SetValue(instance, value);
            }

            return this;
        }

        /// <summary>
        /// Configures a property setter for the specified property of the entity.
        /// </summary>
        /// <typeparam name="TProp">The type of the property to configure.</typeparam>
        /// <param name="expr">An expression identifying the property to configure. The property must have a setter.</param>
        /// <param name="value">The value to assign to the property during configuration.</param>
        /// <returns>An <see cref="IFixtureConfigurator{TEntity}"/> instance for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithSetter<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value)
        {
            if (!ExpressionHelpers.IsPropertyWritable(expr)) throw new InvalidOperationException($"{typeof(TProp).Name} Does not contain a setter. Please use With or WithField when wanting to set the value of a property without a setter.");
            return WithSetterInternal(expr, value);
        }

        private Fixture<TEntity> WithSetterInternal<TProp>(
            Expression<Func<TEntity, TProp>> expr,
            TProp value)
        {
            _fixture ??= (TEntity)InstantiationHelpers.GetInstantiatedInstance(typeof(TEntity), instantiateMembers: true);

            var (instance, property) = ExpressionHelpers.ResolvePropertyPath(_fixture, expr);

            property.SetValue(instance, value);

            return this;
        }

        /// <summary>
        /// Configures the fixture by setting the value of the specified property.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="expr">An expression that identifies the property to set.</param>
        /// <param name="value">The value to assign to the property.</param>
        /// <returns>The fixture configurator for method chaining.</returns>
        /// <remarks>
        /// Automatically determines whether to use the property setter (if writable) or directly set the backing field (if read-only).
        /// </remarks>
        IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.With<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value)
        {
            return WithInternal(expr, value);
        }

        private Fixture<TEntity> WithInternal<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value)
        {
            _fixture ??= (TEntity)InstantiationHelpers.GetInstantiatedInstance(typeof(TEntity), instantiateMembers: true);

            if (ExpressionHelpers.IsPropertyWritable(expr)) return WithSetterInternal(expr, value);
            else return WithFieldInternal(expr, value);
        }
    }
}
