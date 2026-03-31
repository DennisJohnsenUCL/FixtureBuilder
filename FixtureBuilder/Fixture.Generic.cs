using System.Collections;
using System.Linq.Expressions;
using FixtureBuilder.Constructors;
using FixtureBuilder.Extensions;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.Helpers;
using FixtureBuilder.TypeLinks;
using FixtureBuilder.TypeLinks.TypeLinkBuilders;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.UninitializedProviders.UninitializedProviderBuilders;
using FixtureBuilder.ValueConverters;
using FixtureBuilder.ValueConverters.ConverterBuilders;

namespace FixtureBuilder
{
    internal class Fixture<TEntity> : IFixtureConstructor<TEntity>, IFixtureConfigurator<TEntity> where TEntity : class
    {
        private readonly IFixtureContext _context;
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

            _context = InitializeContext();
        }

        internal Fixture(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            if (entity.GetType().GetGenericTypeDefinitionOrDefault() == typeof(Fixture<>)) throw new InvalidOperationException("Please do not use FixtureBuilder to instantiate FixtureBuilder.");

            _context = InitializeContext();
            _fixture = entity;
        }

        /// <summary>
        /// Creates an instance of the entity type <typeparamref name="TEntity"/> without invoking its constructor.
        /// </summary>
        /// <remarks>This method bypasses the constructor of <typeparamref name="TEntity"/> to create an uninitialized
        /// instance without initializing members.</remarks>
        /// <returns>An <see cref="IFixtureConfigurator{TEntity}"/> instance for further configuration of the created entity.</returns>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<TEntity> IFixtureConstructor<TEntity>.CreateUninitialized()
            => ((IFixtureConstructor<TEntity>)this).CreateUninitialized(InitializeMembers.None);

        /// <summary>
        /// Creates an instance of the entity type <typeparamref name="TEntity"/> without invoking its constructor.
        /// </summary>
        /// <remarks>This method bypasses the constructor of <typeparamref name="TEntity"/> to create an uninitialized
        /// instance. After instantiation, the members of the instance can be initialized using providers.</remarks>
        /// <returns>An <see cref="IFixtureConfigurator{TEntity}"/> instance for further configuration of the created entity.</returns>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<TEntity> IFixtureConstructor<TEntity>.CreateUninitialized(InitializeMembers initializeMembers)
        {
            var request = new FixtureRequest(typeof(TEntity));
            var instance = _context.ResolveUninitialized(request, initializeMembers, _context)
                ?? throw new InvalidOperationException($"Failed to intantiate {typeof(TEntity).Name} uninitialized.");

            _fixture = (TEntity)instance;

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
            var request = new FixtureRequest(typeof(TEntity));
            var constructor = new ConstructingProvider();
            var instance = constructor.Resolve(request, args);

            _fixture = (TEntity)instance;

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
        /// <remarks>
        /// This method initializes the fixture instance if it has not already been created.
        /// <br/>Use this method to configure fields that are not accessible through public properties.
        /// <br/>This method supports the MemberLens VS extension which will provide field names automatically if installed.
        /// </remarks>
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

            ExpressionHelper.ValidateExpression(expr);

            var (parentInstance, property) = ExpressionHelper.ResolvePropertyPath(_fixture, expr, _context);
            var instance = ExpressionHelper.InitializePropertyValue(parentInstance, property, _context);
            var propertyType = property.PropertyType;

            return WithFieldInternal(fieldName, propertyType, value, instance);
        }

        private Fixture<TEntity> WithFieldInternal(string fieldName, Type propertyType, object? value, object instance)
        {
            if (!FieldHelper.TryGetField(propertyType, fieldName, out var fieldInfo))
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
        IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithBackingField<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value)
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

        /// <summary>
        /// Configures the fixture by setting the value of a field backing the specified property without enforcing the type of the property.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="expr">An expression that identifies the property whose backing field should be set.</param>
        /// <param name="value">The value to assign to the backing field.</param>
        /// <returns>The fixture configurator for method chaining.</returns>
        /// <remarks>
        /// Use this method when the backing field for the property has a different, un-assignable type from the property.
        /// The backing field name is automatically discovered using common naming conventions.
        /// Supports both regular properties and interface-implemented properties with explicit backing fields.
        /// </remarks>
        IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithBackingFieldUntyped<TProp>(Expression<Func<TEntity, TProp>> expr, object? value)
            => WithBackingFieldInternal(expr, value);

        /// <summary>
        /// Configures the fixture by setting the value of a specifically named field backing the specified property without enforcing the type of the property.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="fieldName">The explicit name of the backing field to set.</param>
        /// <param name="expr">An expression that identifies the property whose backing field should be set.</param>
        /// <param name="value">The value to assign to the backing field.</param>
        /// <returns>The fixture configurator for method chaining.</returns>
        /// <remarks>
        /// Use this method when the backing field for the property has a different, un-assignable type from the property.
        /// Use this overload when you need to specify a non-standard backing field name that cannot be automatically discovered.
        /// </remarks>
        IFixtureConfigurator<TEntity> IFixtureConfigurator<TEntity>.WithBackingFieldUntyped<TProp>(string fieldName, Expression<Func<TEntity, TProp>> expr, object? value)
            => WithBackingFieldInternal(expr, value, fieldName);

        private Fixture<TEntity> WithBackingFieldInternal<TProp>(
            Expression<Func<TEntity, TProp>> expr,
            object? value,
            string? fieldName = null)
        {
            _fixture ??= InstantiateFixture();

            ExpressionHelper.ValidateExpression(expr);

            var (instance, property) = ExpressionHelper.ResolvePropertyPath(_fixture, expr, _context);

            var propertyParentType = instance.GetType();

            if (!FieldHelper.TryGetPropertyBackingField(propertyParentType, property, fieldName, out var backingField))
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
                var collection = _context.Convert(fieldType, value, _context)!;
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

            ExpressionHelper.ValidateExpression(expr);
            ExpressionHelper.ValidatePropertyWriteable(expr);

            var (instance, property) = ExpressionHelper.ResolvePropertyPath(_fixture, expr, _context);

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

            if (ExpressionHelper.IsPropertyWritable(expr)) return WithSetterInternal(expr, value);
            else return WithBackingFieldInternal(expr, value);
        }

        private static TEntity InstantiateFixture()
        {
            //TODO: Autoconstructor
            return (TEntity)InstantiationHelper.GetInstantiatedInstance(typeof(TEntity));
        }

        private static void ValidateNullableValueTypeAssignment(Type type, object? value)
        {
            if (value == null && type.IsValueType && !(type.GetGenericTypeDefinitionOrDefault() == typeof(Nullable<>)))
                throw new InvalidOperationException("Cannot assign null to a non-nullable value type. Consider passing default instead.");
        }

        private static IFixtureContext InitializeContext()
        {
            var converter = new Func<IValueConverter>(() => new ConverterFactory().CreateDefaultConverter());
            var typeLink = new Func<ITypeLink>(() => new TypeLinkFactory().CreateDefaultTypeLink());
            var uninitializedProvider = new Func<IFixtureUninitializedProvider>(() => new UninitializedProviderFactory().CreateDefaultUninitializedProvider());
            var resolver = new LazyContextResolver(converter, typeLink, uninitializedProvider);
            var context = new FixtureContext(resolver) as IFixtureContext;
            return context;
        }
    }
}
