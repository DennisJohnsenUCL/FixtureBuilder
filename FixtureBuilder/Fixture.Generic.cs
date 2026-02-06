using FixtureBuilder.Extensions;
using FixtureBuilder.Helpers;
using FixtureBuilder.TypeLinkers;
using FixtureBuilder.ValueConverters;
using System.Collections;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

namespace FixtureBuilder
{
    internal class Fixture<TEntity> : IFixtureConstructor<TEntity>, IFixtureConfigurator<TEntity> where TEntity : class
    {
        private IValueConverter? _converter;
        private TEntity _fixture = null!;

        /// <summary>
        /// Builds and returns the configured fixture instance of the specified type.
        /// </summary>
        /// <remarks>If the fixture instance has not been previously created, a new instance is instantiated  with its
        /// members initialized. Subsequent calls will return the same instance.</remarks>
        /// <returns>The configured fixture instance of type <typeparamref name="TEntity"/>.</returns>
        TEntity IFixtureConfigurator<TEntity>.Build()
        {
            _fixture ??= InstantiateFixture();

            return _fixture;
        }

        internal Fixture()
        {
            if (typeof(TEntity).IsInterface)
                throw new InvalidOperationException($"Cannot create fixtures of interface types: {typeof(TEntity).Name}. Please use concrete types for fixtures.");

            if (typeof(TEntity).IsAbstract)
                throw new InvalidOperationException($"Cannot create fixtures of abstract types: {typeof(TEntity).Name}. Please use concrete types for fixtures.");

            if (typeof(TEntity).GetGenericTypeDefinitionOrDefault() == typeof(Fixture<>)) throw new InvalidOperationException("Please do not use FixtureBuilder to instantiate FixtureBuilder.");
        }

        internal Fixture(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity), $"Cannot use a null instance as fixture {typeof(TEntity).Name}. Please use generic parameter instead for generating new fixtures.");
            if (entity.GetType().GetGenericTypeDefinitionOrDefault() == typeof(Fixture<>)) throw new InvalidOperationException("Please do not use FixtureBuilder to instantiate FixtureBuilder.");

            _fixture = entity;
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
            _fixture ??= InstantiateFixture();

            if (_fixture is not TTarget target)
                throw new InvalidCastException($"Cannot cast {typeof(TEntity).Name} to {typeof(TTarget).Name}.");

            return new Fixture<TTarget>(target);
        }

        /// <summary>
        /// Configures the fixture by setting the specified field to the given value.
        /// </summary>
        /// <remarks>This method initializes the fixture instance if it has not already been created.
        /// Use this method to configure fields that are not accessible through public properties.</remarks>
        /// <param name="fieldName">The name of the field to set. This must match the name of an existing field in the entity.</param>
        /// <param name="value">The value to assign to the specified field. The value must be compatible with the field's type.</param>
        /// <returns>The current <see cref="IFixtureConfigurator{TEntity}"/> instance, allowing for method chaining.</returns>
        IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithField<T>(string fieldName, T value)
        {
            _fixture ??= InstantiateFixture();

            return WithFieldInternal(fieldName, typeof(TEntity), value, _fixture);
        }

        /// <summary>
        /// Configures the fixture by setting the specified field to the given value, on the specified nested property.
        /// </summary>
        /// <remarks>This method initializes the fixture instance if it has not already been created.
        /// Use this method to configure fields that are not accessible through public properties.</remarks>
        /// <param name="fieldName">The name of the field to set. This must match the name of an existing field in the entity.</param>
        /// <param name="expr">The nested property on which to set the field.</param>
        /// <param name="value">The value to assign to the specified field. The value must be compatible with the field's type.</param>
        /// <returns>The current <see cref="IFixtureConfigurator{TEntity}"/> instance, allowing for method chaining.</returns>
        IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithField<T>(string fieldName, Expression<Func<TEntity, object?>> expr, T value)
        {
            _fixture ??= InstantiateFixture();

            ExpressionHelpers.ValidateExpression(expr);

            var (instance, property) = ExpressionHelpers.ResolvePropertyPath(_fixture, expr, instantiateTarget: true);
            var propertyType = property.PropertyType;

            return WithFieldInternal(fieldName, propertyType, value, instance);
        }

        private Fixture<TEntity> WithFieldInternal(string fieldName, Type propertyType, object? value, object instance)
        {
            if (!FieldHelpers.TryGetField(propertyType, fieldName, out var fieldInfo))
                throw new InvalidOperationException($"Field '{fieldName}' not found on {propertyType.Name}.");

            var fieldType = fieldInfo.FieldType;

            ValidateNullableValueTypeAssignment(fieldType, value);

            var sourceType = value?.GetType();
            if (sourceType == null || fieldType == sourceType || fieldType.IsAssignableFrom(sourceType))
            {
                fieldInfo.SetValue(instance, value);
            }
            else throw new InvalidOperationException($"Cannot assign value of type {sourceType.Name} to field of type {fieldType.Name}");

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
        IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithBackingField<TProp>(
            Expression<Func<TEntity, TProp>> expr,
            TProp value)
            => WithBackingFieldInternal(expr, value);

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
        IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithBackingField<TProp>(string fieldName, Expression<Func<TEntity, TProp>> expr, TProp value)
            => WithBackingFieldInternal(expr, value, fieldName);

        private Fixture<TEntity> WithBackingFieldInternal<TProp>(
            Expression<Func<TEntity, TProp>> expr,
            TProp value,
            string? fieldName = null)
        {
            _fixture ??= InstantiateFixture();

            ExpressionHelpers.ValidateExpression(expr);

            var (instance, property) = ExpressionHelpers.ResolvePropertyPath(_fixture, expr, instantiateTarget: false);

            var propertyParentType = instance.GetType();

            if (!FieldHelpers.TryGetPropertyBackingField(propertyParentType, property, fieldName, out var backingField))
                throw new InvalidOperationException($"Backing field not found for property {property.Name}. Please specify the name of the backing field if not following standard naming.");

            var fieldType = backingField.FieldType;

            ValidateNullableValueTypeAssignment(fieldType, value);

            var sourceType = value?.GetType();

            if (sourceType == null || fieldType == sourceType || fieldType.IsAssignableFrom(sourceType))
            {
                backingField.SetValue(instance, value);
            }
            else if (fieldType != typeof(string)
                && value != null
                && typeof(IEnumerable).IsAssignableFrom(fieldType)
                && typeof(IEnumerable).IsAssignableFrom(sourceType))
            {
                IEnumerable collection;
                _converter ??= InitializeConverter();
                try
                {
                    collection = (IEnumerable)_converter.Convert(fieldType, value)!;
                    backingField.SetValue(instance, collection);
                    return this;
                }
                catch { }

                if (CollectionHelpers.IsDictionary(fieldType)) collection = CollectionHelpers.CastToDictionary(fieldType, (IEnumerable)value);
                else collection = CollectionHelpers.CastToCollection(fieldType, (IEnumerable)value);

                backingField.SetValue(instance, collection);
            }
            else throw new InvalidOperationException($"Cannot assign type {sourceType.Name} to backing field for property {property.Name}");

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
            => WithSetterInternal(expr, value);

        private Fixture<TEntity> WithSetterInternal<TProp>(
            Expression<Func<TEntity, TProp>> expr,
            TProp value)
        {
            _fixture ??= InstantiateFixture();

            ExpressionHelpers.ValidateExpression(expr);
            ExpressionHelpers.ValidatePropertyWriteable(expr);

            var (instance, property) = ExpressionHelpers.ResolvePropertyPath(_fixture, expr, instantiateTarget: false);

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
            _fixture ??= InstantiateFixture();

            if (ExpressionHelpers.IsPropertyWritable(expr)) return WithSetterInternal(expr, value);
            else return WithBackingFieldInternal(expr, value);
        }

        /// <summary>
        /// Determines whether a field with the specified name exists on the <typeparamref name="TEntity"/> type.
        /// </summary>
        /// <param name="fieldName">The name of the field to search for.</param>
        /// <returns><see langword="true"/> if the field exists on the fixture type, <see langword="false"/> if not.</returns>
        bool IFixtureConfigurator<TEntity>.HasField(string fieldName)
        {
            return FieldHelpers.TryGetField(typeof(TEntity), fieldName, out var _);
        }

        /// <summary>
        /// Determines whether a field with the specified name exists on the given member of the <typeparamref name="TEntity"/> type.
        /// </summary>
        /// <param name="fieldName">The name of the field to search for.</param>
        /// <param name="expr">The member to search for the field on.</param>
        /// <returns><see langword="true"/> if the field exists on the member of the fixture type, <see langword="false"/> if not.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        bool IFixtureConfigurator<TEntity>.HasField(string fieldName, Expression<Func<TEntity, object?>> expr)
        {
            ExpressionHelpers.ValidateExpression(expr);

            if (expr.Body is MemberExpression me && me.Member is PropertyInfo pi)
                return FieldHelpers.TryGetField(pi.PropertyType, fieldName, out var _);

            return false;
        }

        private static TEntity InstantiateFixture()
        {
            return (TEntity)InstantiationHelpers.GetInstantiatedInstance(typeof(TEntity), instantiateMembers: true);
        }

        private static void ValidateNullableValueTypeAssignment(Type type, object? value)
        {
            if (value == null && type.IsValueType && !(type.GetGenericTypeDefinitionOrDefault() == typeof(Nullable<>)))
                throw new InvalidOperationException("Cannot assign null to a non-nullable value type. Consider passing default instead.");
        }

        private static IValueConverter InitializeConverter()
        {
            var converter = new ThrowingConverter(
                new TypeLinkingConverter(
                    new EnumerableElementCastingConverter(
                        new CompositeConverter([
                            new MutableGenericCollectionConverter(),
                            new ImmutableCollectionConverter(),
                            new FrozenSetConverter(),
                            new ArrayConverter()])),
                    new CompositeTypeLink([
                        new TypeLink(typeof(IEnumerable<>), typeof(List<>)),
                        new TypeLink(typeof(IList<>), typeof(List<>)),
                        new TypeLink(typeof(IReadOnlyList<>), typeof(List<>)),
                        new TypeLink(typeof(ICollection<>), typeof(List<>)),
                        new TypeLink(typeof(IReadOnlyCollection<>), typeof(List<>)),
                        new TypeLink(typeof(ISet<>), typeof(HashSet<>)),
                        new TypeLink(typeof(IReadOnlySet<>), typeof(HashSet<>)),
                        new TypeLink(typeof(IImmutableList<>), typeof(ImmutableList<>)),
                        new TypeLink(typeof(IImmutableStack<>), typeof(ImmutableStack<>)),
                        new TypeLink(typeof(IImmutableQueue<>), typeof(ImmutableQueue<>)),
                        new TypeLink(typeof(IImmutableSet<>), typeof(ImmutableHashSet<>))]))) as IValueConverter;

            return converter;
        }
    }
}
