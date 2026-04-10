using System.Linq.Expressions;
using System.Reflection;
using FixtureBuilder.ConstructingProviders;
using FixtureBuilder.Extensions;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.Helpers;
using FixtureBuilder.MemberInstantiators;
using FixtureBuilder.UninitializedProviders;

namespace FixtureBuilder
{
    internal class Fixture<T> : IFixtureConstructor<T>, IFixtureConfigurator<T> where T : class
    {
        private readonly IFixtureContext _context;
        private T _fixture = null!;

        /// <summary>
        /// Builds and returns the configured fixture instance of the specified type.
        /// </summary>
        /// <remarks>If the fixture instance has not been previously created, a new instance is instantiated  with its
        /// members initialized. Subsequent calls will return the same instance.</remarks>
        /// <returns>The configured fixture instance of type <typeparamref name="T"/>.</returns>
        T IFixtureConfigurator<T>.Build()
        {
            _fixture ??= InstantiateFixture();

            return _fixture;
        }

        internal Fixture()
        {
            if (typeof(T).IsInterface)
                throw new InvalidOperationException($"Cannot create fixtures of interface types: {typeof(T).Name}. Please use concrete types for fixtures.");

            if (typeof(T).IsAbstract)
                throw new InvalidOperationException($"Cannot create fixtures of abstract types: {typeof(T).Name}. Please use concrete types for fixtures.");

            if (typeof(T).GetGenericTypeDefinitionOrDefault() == typeof(Fixture<>)) throw new InvalidOperationException("Please do not use FixtureBuilder to instantiate FixtureBuilder.");

            _context = InitializeContext();
        }

        internal Fixture(T instance)
        {
            ArgumentNullException.ThrowIfNull(instance);
            if (instance.GetType().GetGenericTypeDefinitionOrDefault() == typeof(Fixture<>)) throw new InvalidOperationException("Please do not use FixtureBuilder to instantiate FixtureBuilder.");

            _context = InitializeContext();
            _fixture = instance;
        }

        /// <summary>
        /// Creates an instance of the entity type <typeparamref name="T"/> without invoking its constructor.
        /// </summary>
        /// <remarks>This method bypasses the constructor of <typeparamref name="T"/> to create an uninitialized
        /// instance without initializing members.</remarks>
        /// <returns>An <see cref="IFixtureConfigurator{T}"/> instance for further configuration of the created entity.</returns>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<T> IConstructor<IFixtureConfigurator<T>>.CreateUninitialized()
            => ((IFixtureConstructor<T>)this).CreateUninitialized(_context.Options.DefaultInitializeMembers);

        /// <summary>
        /// Creates an instance of the entity type <typeparamref name="T"/> without invoking its constructor.
        /// </summary>
        /// <remarks>This method bypasses the constructor of <typeparamref name="T"/> to create an uninitialized
        /// instance. After instantiation, the members of the instance can be initialized using providers.</remarks>
        /// <returns>An <see cref="IFixtureConfigurator{T}"/> instance for further configuration of the created entity.</returns>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<T> IConstructor<IFixtureConfigurator<T>>.CreateUninitialized(InitializeMembers initializeMembers)
        {
            var request = new FixtureRequest(typeof(T));
            var instance = _context.ResolveUninitialized(request, initializeMembers, _context)
                ?? throw new InvalidOperationException($"Failed to intantiate {typeof(T).Name} uninitialized.");

            _fixture = (T)instance;

            return this;
        }

        /// <summary>
        /// Configures the fixture to use a specific constructor for creating an instance of the entity.
        /// </summary>
        /// <param name="args">The arguments to pass to the constructor. The arguments must match the constructor's parameter types and order.</param>
        /// <returns>An <see cref="IFixtureConfigurator{T}"/> instance for further configuration.</returns>
        /// <exception cref="MissingMethodException"/>
        IFixtureConfigurator<T> IConstructor<IFixtureConfigurator<T>>.UseConstructor(params object[] arguments)
        {
            var request = new FixtureRequest(typeof(T));
            var constructor = new ConstructingProvider();
            var instance = constructor.ResolveWithArguments(request, arguments);

            _fixture = (T)instance;

            return this;
        }

        /// <summary>
        /// Creates an instance of the entity type <typeparamref name="T"/> by automatically selecting
        /// and invoking a constructor and resolving all dependencies recursively.
        /// </summary>
        /// <remarks>This method selects the simplest available constructor and resolves each parameter
        /// through the fixtures resolution pipelinel.</remarks>
        /// <returns>An <see cref="IFixtureConfigurator{T}"/> instance for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<T> IConstructor<IFixtureConfigurator<T>>.UseAutoConstructor()
        {
            var request = new FixtureRequest(typeof(T));
            var instance = _context.AutoResolve(request, _context);

            _fixture = (T)instance;

            return this;
        }

        /// <summary>
        /// Casts the current fixture to the specified target type.
        /// </summary>
        /// <typeparam name="TTarget">The type to which the fixture should be cast.</typeparam>
        /// <returns>An <see cref="IFixtureConfigurator{TTarget}"/> instance for the casted fixture.</returns>
        /// <exception cref="InvalidCastException"/>
        IFixtureConfigurator<TTarget> IFixtureConfigurator<T>.CastTo<TTarget>()
        {
            _fixture ??= InstantiateFixture();

            if (_fixture is not TTarget target)
                throw new InvalidCastException($"Cannot cast {typeof(T).Name} to {typeof(TTarget).Name}.");

            return new Fixture<TTarget>(target);
        }

        /// <summary>
        /// Instantiates the specified property using a default instantiation method and assigns it to the fixture.
        /// </summary>
        /// <param name="expr">An expression identifying the property to instantiate. Intermediate properties in the chain are automatically initialized if allowed.</param>
        /// <remarks>
        /// The default instantiation method can be set through via the DefaultInstantiateInstantiationMethod option.
        /// If the property has a setter, the value is assigned through it; otherwise, the backing field is set directly.
        /// </remarks>
        /// <returns>An <see cref="IFixtureConfigurator{T}"/> instance for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<T> IFixtureConfigurator<T>.Instantiate<TProp>(Expression<Func<T, TProp>> expr)
        {
            var instance = (TProp)_context.InstantiateWithStrategy(new FixtureRequest(typeof(TProp)), _context.Options.DefaultInstantiateInstantiationMethod, InitializeMembers.None);

            return InstantiateInternal(expr, instance);
        }

        /// <summary>
        /// Instantiates the specified property using a caller-defined instantiation strategy and assigns it to the fixture.
        /// </summary>
        /// <param name="expr">An expression identifying the property to instantiate. Intermediate properties in the chain are automatically initialized.</param>
        /// <param name="func">A function that receives a <see cref="MemberInstantiator{TProp}"/> and returns an instance of <typeparamref name="TProp"/>.
        /// The instantiator provides access to construction strategies such as <c>UseAutoConstructor</c>, <c>UseConstructor</c>, and <c>CreateUninitialized</c>.</param>
        /// <returns>An <see cref="IFixtureConfigurator{T}"/> instance for further configuration.</returns>
        /// <remarks>
        /// If the property has a setter, the value is assigned through it; otherwise, the backing field is set directly.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<T> IFixtureConfigurator<T>.Instantiate<TProp>(Expression<Func<T, TProp>> expr, Func<IConstructor<TProp>, TProp> func)
        {
            var instance = func(new MemberInstantiator<TProp>(_context));

            return InstantiateInternal(expr, instance);
        }

        private Fixture<T> InstantiateInternal<TProp>(Expression<Func<T, TProp>> expr, TProp instance)
        {
            _fixture ??= InstantiateFixture();

            ExpressionHelper.ValidateExpression(expr);
            ExpressionHelper.ResolvePropertyParent(_fixture, expr, _context);

            ((IFixtureConfigurator<T>)this).With(expr, instance);

            return this;
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
        /// <returns>The current <see cref="IFixtureConfigurator{T}"/> instance, allowing for method chaining.</returns>
        IFixtureConfigurator<T> IFixtureConfigurator<T>.WithField<TValue>(string fieldName, TValue value)
        {
            _fixture ??= InstantiateFixture();

            return WithFieldInternal(fieldName, typeof(T), value, _fixture);
        }

        /// <summary>
        /// Configures the fixture by setting the specified field to the given value, on the specified nested property.
        /// </summary>
        /// <remarks>
        /// This method initializes the fixture instance if it has not already been created.
        /// <br/>Use this method to configure fields that are not accessible through public properties.
        /// <br/>This method supports the MemberLens VS extension which will provide field names automatically if installed.
        /// </remarks>
        /// <param name="fieldName">The name of the field to set. This must match the name of an existing field in the entity.</param>
        /// <param name="expr">The nested property on which to set the field.</param>
        /// <param name="value">The value to assign to the specified field. The value must be compatible with the field's type.</param>
        /// <returns>The current <see cref="IFixtureConfigurator{T}"/> instance, allowing for method chaining.</returns>
        IFixtureConfigurator<T> IFixtureConfigurator<T>.WithField<TProp, TValue>(Expression<Func<T, TProp>> expr, string fieldName, TValue value)
        {
            _fixture ??= InstantiateFixture();

            ExpressionHelper.ValidateExpression(expr);

            var (instance, property) = ExpressionHelper.ResolvePropertyInstance(_fixture, expr, _context);
            var propertyType = property.PropertyType;

            return WithFieldInternal(fieldName, propertyType, value, instance);
        }

        private Fixture<T> WithFieldInternal(string fieldName, Type propertyType, object? value, object instance)
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
        IFixtureConfigurator<T> IFixtureConfigurator<T>.WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value)
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
        IFixtureConfigurator<T> IFixtureConfigurator<T>.WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value, string fieldName)
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
        IFixtureConfigurator<T> IFixtureConfigurator<T>.WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value)
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
        IFixtureConfigurator<T> IFixtureConfigurator<T>.WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value, string fieldName)
            => WithBackingFieldInternal(expr, value, fieldName);

        private Fixture<T> WithBackingFieldInternal<TProp>(
            Expression<Func<T, TProp>> expr,
            object? value,
            string? fieldName = null)
        {
            _fixture ??= InstantiateFixture();

            ExpressionHelper.ValidateExpression(expr);
            var (instance, property) = ExpressionHelper.ResolvePropertyParent(_fixture, expr, _context);
            var propertyParentType = instance.GetType();

            if (!FieldHelper.TryGetPropertyBackingField(propertyParentType, property, fieldName, out var backingField))
                throw new InvalidOperationException($"Backing field not found for property {property.Name}. Please specify the name of the backing field if not following standard naming.");

            var fieldType = backingField.FieldType;

            ValidateNullableValueTypeAssignment(fieldType, value);

            var sourceType = value?.GetType();
            if (sourceType != null && fieldType != sourceType && !fieldType.IsAssignableFrom(sourceType))
            {
                value = _context.Convert(fieldType, value!, _context)
                    ?? throw new InvalidOperationException($"Cannot assign type {sourceType.Name} to backing field for property {property.Name}, or convert it to a fitting type.");
            }
            backingField.SetValue(instance, value);
            return this;
        }

        /// <summary>
        /// Configures a property setter for the specified property of the entity.
        /// </summary>
        /// <typeparam name="TProp">The type of the property to configure.</typeparam>
        /// <param name="expr">An expression identifying the property to configure. The property must have a setter.</param>
        /// <param name="value">The value to assign to the property during configuration.</param>
        /// <returns>An <see cref="IFixtureConfigurator{T}"/> instance for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<T> IFixtureConfigurator<T>.WithSetter<TProp>(Expression<Func<T, TProp>> expr, TProp value)
            => WithSetterInternal(expr, value);

        private Fixture<T> WithSetterInternal<TProp>(
            Expression<Func<T, TProp>> expr,
            TProp value)
        {
            _fixture ??= InstantiateFixture();

            ExpressionHelper.ValidateExpression(expr);
            ExpressionHelper.ValidatePropertyWriteable(expr);

            var (instance, property) = ExpressionHelper.ResolvePropertyParent(_fixture, expr, _context);

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
        IFixtureConfigurator<T> IFixtureConfigurator<T>.With<TProp>(Expression<Func<T, TProp>> expr, TProp value)
        {
            _fixture ??= InstantiateFixture();

            if (ExpressionHelper.IsPropertyWritable(expr)) return WithSetterInternal(expr, value);
            else return WithBackingFieldInternal(expr, value);
        }

        /// <summary>
        /// Invokes a method on the fixture or its nested properties via an expression.
        /// Intermediate properties in the chain are automatically initialized.
        /// </summary>
        /// <param name="expr">A lambda expression representing a method call on the fixture, e.g. <c>x => x.Child.DoSomething()</c>.</param>
        /// <returns>The configurator for further chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the expression is not a valid method call on a property access chain.</exception>
        IFixtureConfigurator<T> IFixtureConfigurator<T>.Invoke(Expression<Action<T>> expr)
        {
            _fixture ??= InstantiateFixture();

            ExpressionHelper.ValidateExpression(expr);
            ExpressionHelper.ResolveMethodParent(_fixture, expr, _context);

            var action = expr.Compile();
            action.Invoke(_fixture);

            return this;
        }

        /// <summary>
        /// Invokes a method on the fixture by name using reflection, allowing access to private and protected methods.
        /// </summary>
        /// <remarks>
        /// This method supports the MemberLens VS extension which will provide method names automatically if installed.
        /// </remarks>
        /// <param name="methodName">The name of the method to invoke on the fixture.</param>
        /// <param name="arguments">Arguments to pass to the method.</param>
        /// <returns>The configurator for further chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the method is not found on <typeparamref name="T"/>.</exception>
        IFixtureConfigurator<T> IFixtureConfigurator<T>.InvokePrivate(string methodName, params object?[] arguments)
        {
            _fixture ??= InstantiateFixture();

            InvokePrivateInternal(typeof(T), _fixture, methodName, arguments);

            return this;
        }

        /// <summary>
        /// Invokes a method by name on a nested property of the fixture, allowing access to private and protected methods.
        /// The property and any intermediate properties in the chain are automatically initialized.
        /// </summary>
        /// <remarks>
        /// This method supports the MemberLens VS extension which will provide method names automatically if installed.
        /// </remarks>
        /// <typeparam name="TProp">The type of the target property.</typeparam>
        /// <param name="expr">A lambda expression representing the property path, e.g. <c>x => x.Child.Nested</c>.</param>
        /// <param name="methodName">The name of the method to invoke on the resolved property.</param>
        /// <param name="arguments">Arguments to pass to the method.</param>
        /// <returns>The configurator for further chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the expression is invalid or the method is not found on <typeparamref name="TProp"/>.</exception>
        IFixtureConfigurator<T> IFixtureConfigurator<T>.InvokePrivate<TProp>(Expression<Func<T, TProp>> expr, string methodName, params object?[] arguments)
        {
            _fixture ??= InstantiateFixture();

            ExpressionHelper.ValidateExpression(expr);

            var (instance, property) = ExpressionHelper.ResolvePropertyInstance(_fixture, expr, _context);
            var propertyType = property.PropertyType;

            InvokePrivateInternal(propertyType, instance, methodName, arguments);

            return this;
        }

        private static void InvokePrivateInternal(Type parentType, object instance, string methodName, params object?[] args)
        {
            parentType.InvokeMember(
                methodName,
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                instance,
                args);
        }

        internal T InstantiateFixture()
        {
            if (!_context.Options.AllowImplicitConstruction)
                throw new InvalidOperationException($"Skipping instantiation step is not allowed. " +
                    $"Please use UseAutoConstructor, UseConstructor, or CreateUninitialized before calling any configuration methods. " +
                    $"Explicit instantiation can be allowed via AllowImplicitConstruction option.");

            return (T)_context.InstantiateWithStrategy(new FixtureRequest(typeof(T)), _context.Options.DefaultInstantiationMethod, _context.Options.DefaultInitializeMembers);
        }

        private static void ValidateNullableValueTypeAssignment(Type type, object? value)
        {
            if (value == null && type.IsValueType && !(type.GetGenericTypeDefinitionOrDefault() == typeof(Nullable<>)))
                throw new InvalidOperationException("Cannot assign null to a non-nullable value type. Consider passing default instead.");
        }

        private static IFixtureContext InitializeContext()
        {
            return FixtureContextFactory.CreateLazyContext();
        }
    }
}
