using System.Linq.Expressions;
using System.Reflection;
using FixtureBuilder.Configuration;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.UninitializedProviders;
using FixtureBuilder.Extensions;

namespace FixtureBuilder
{
    internal class Fixture<T> : IFixtureConstructor<T>, IFixtureConfigurator<T> where T : class
    {
        private readonly IFixtureContext _context;
        private readonly ExpressionResolver _expressionResolver;
        private T _fixture = null!;

        T IFixtureConfigurator<T>.Build()
        {
            _fixture ??= InstantiateFixture();

            return _fixture;
        }

        internal Fixture() : this(InitializeContext()) { }

        internal Fixture(IFixtureContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (typeof(T).IsInterface)
                throw new InvalidOperationException($"Cannot create fixtures of interface types: {typeof(T).Name}. Please use concrete types for fixtures.");

            if (typeof(T).IsAbstract)
                throw new InvalidOperationException($"Cannot create fixtures of abstract types: {typeof(T).Name}. Please use concrete types for fixtures.");

            if (typeof(T).GetGenericTypeDefinitionOrDefault() == typeof(Fixture<>)) throw new InvalidOperationException("Please do not use FixtureBuilder to instantiate FixtureBuilder.");

            _context = context;
            _expressionResolver = InitializeExpressionResolver();
        }

        internal Fixture(T instance) : this(instance, InitializeContext()) { }

        internal Fixture(T instance, IFixtureContext context)
        {
            ArgumentNullException.ThrowIfNull(instance);
            ArgumentNullException.ThrowIfNull(context);

            if (instance.GetType().GetGenericTypeDefinitionOrDefault() == typeof(Fixture<>)) throw new InvalidOperationException("Please do not use FixtureBuilder to instantiate FixtureBuilder.");

            _context = context;
            _expressionResolver = InitializeExpressionResolver();
            _fixture = instance;
        }

        IFixtureConfigurator<T> IConstructor<IFixtureConfigurator<T>>.CreateUninitialized()
            => ((IFixtureConstructor<T>)this).CreateUninitialized(_context.OptionsFor(typeof(T)).DefaultInitializeMembers);

        IFixtureConfigurator<T> IConstructor<IFixtureConfigurator<T>>.CreateUninitialized(InitializeMembers initializeMembers)
        {
            var request = new FixtureRequest(typeof(T));
            var instance = _context.UninitializedProvider.ResolveUninitialized(request, initializeMembers, _context);
            if (instance is NoResult || instance == null)
                throw new InvalidOperationException($"Failed to intantiate {typeof(T).Name} uninitialized.");

            _fixture = (T)instance;

            return this;
        }

        IFixtureConfigurator<T> IConstructor<IFixtureConfigurator<T>>.UseConstructor(params object[] arguments)
        {
            var request = new FixtureRequest(typeof(T));
            var instance = _context.ConstructingProvider.ResolveWithArguments(request, arguments);

            _fixture = (T)instance;

            return this;
        }

        IFixtureConfigurator<T> IConstructor<IFixtureConfigurator<T>>.UseAutoConstructor()
        {
            var request = new FixtureRequest(typeof(T));
            var instance = _context.AutoConstructingProvider.AutoResolve(request, _context);

            _fixture = (T)instance;

            return this;
        }

        IFixtureConfigurator<TTarget> IFixtureConfigurator<T>.CastTo<TTarget>()
        {
            _fixture ??= InstantiateFixture();

            if (_fixture is not TTarget target)
                throw new InvalidCastException($"Cannot cast {typeof(T).Name} to {typeof(TTarget).Name}.");

            return new Fixture<TTarget>(target);
        }

        IFixtureConfigurator<T> IFixtureConfigurator<T>.Instantiate<TProp>(Expression<Func<T, TProp>> expr)
        {
            ExpressionValidator.ValidateExpression(expr);

            var source = ((MemberExpression)expr.Body).Member;
            var request = new FixtureRequest(typeof(TProp), source, typeof(T), source.Name);
            var value = (TProp)_context.ProvideWithStrategy(request, _context.OptionsFor(typeof(T)).DefaultInstantiateInstantiationMethod, InitializeMembers.None)!;

            return ((IFixtureConfigurator<T>)this).With(expr, value);
        }

        IFixtureConfigurator<T> IFixtureConfigurator<T>.Instantiate<TProp>(Expression<Func<T, TProp>> expr, Func<IConstructor<TProp>, TProp> func)
        {
            ExpressionValidator.ValidateExpression(expr);

            var source = ((MemberExpression)expr.Body).Member;
            var request = new FixtureRequest(typeof(TProp), source, typeof(T), source.Name);
            var value = func(new MemberInstantiator<TProp>(request, _context));

            return ((IFixtureConfigurator<T>)this).With(expr, value);
        }

        IFixtureConfigurator<T> IFixtureConfigurator<T>.WithField<TValue>(string fieldName, TValue value)
        {
            _fixture ??= InstantiateFixture();

            return WithFieldInternal(fieldName, typeof(T), value, _fixture);
        }

        IFixtureConfigurator<T> IFixtureConfigurator<T>.WithField<TProp, TValue>(Expression<Func<T, TProp>> expr, string fieldName, TValue value)
        {
            _fixture ??= InstantiateFixture();

            ExpressionValidator.ValidateExpression(expr);

            var (instance, dataMember) = _expressionResolver.ResolveDataMemberInstance(_fixture, expr, _context);
            var dataMemberType = dataMember.DataMemberType;

            return WithFieldInternal(fieldName, dataMemberType, value, instance);
        }

        private Fixture<T> WithFieldInternal(string fieldName, Type dataMemberType, object? value, object instance)
        {
            if (!dataMemberType.TryGetField(fieldName, out var fieldInfo))
                throw new InvalidOperationException($"Field '{fieldName}' not found on {dataMemberType.Name}.");

            FieldHelper.SetFieldValue(fieldInfo, instance, value);

            return this;
        }

        IFixtureConfigurator<T> IFixtureConfigurator<T>.WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value)
            => WithBackingFieldInternal(expr, value);

        IFixtureConfigurator<T> IFixtureConfigurator<T>.WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value, string fieldName)
            => WithBackingFieldInternal(expr, value, fieldName);

        IFixtureConfigurator<T> IFixtureConfigurator<T>.WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value)
            => WithBackingFieldInternal(expr, value);

        IFixtureConfigurator<T> IFixtureConfigurator<T>.WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value, string fieldName)
            => WithBackingFieldInternal(expr, value, fieldName);

        private Fixture<T> WithBackingFieldInternal<TProp>(
            Expression<Func<T, TProp>> expr,
            object? value,
            string? fieldName = null)
        {
            _fixture ??= InstantiateFixture();

            ExpressionValidator.ValidatePropertyExpression(expr);
            var (instance, dataMember) = _expressionResolver.ResolveDataMemberParent(_fixture, expr, _context);
            var property = dataMember.Property;
            var propertyParentType = instance.GetType();

            if (!FieldHelper.TryGetPropertyBackingField(propertyParentType, property, fieldName, out var backingField))
                throw new InvalidOperationException($"Backing field not found for property {property.Name}. Please specify the name of the backing field if not following standard naming.");

            FieldHelper.SetBackingFieldValue(backingField, property, instance, value, typeof(T), _context);

            return this;
        }

        IFixtureConfigurator<T> IFixtureConfigurator<T>.WithSetter<TProp>(Expression<Func<T, TProp>> expr, TProp value)
        {
            _fixture ??= InstantiateFixture();

            ExpressionValidator.ValidatePropertyExpression(expr);
            ExpressionValidator.ValidatePropertyWriteable(expr);

            var (instance, dataMember) = _expressionResolver.ResolveDataMemberParent(_fixture, expr, _context);
            var property = dataMember.Property;

            property.SetValue(instance, value);

            return this;
        }

        IFixtureConfigurator<T> IFixtureConfigurator<T>.With<TProp>(Expression<Func<T, TProp>> expr, TProp value)
        {
            _fixture ??= InstantiateFixture();

            ExpressionValidator.ValidateExpression(expr);
            var (instance, dataMember) = _expressionResolver.ResolveDataMemberParent(_fixture, expr, _context);

            if (dataMember.IsFieldInfo)
                FieldHelper.SetFieldValue(dataMember.Field, instance, value);

            else if (ExpressionValidator.IsPropertyWritable(expr))
                ((IFixtureConfigurator<T>)this).WithSetter(expr, value);

            else
                WithBackingFieldInternal(expr, value);

            return this;
        }

        IFixtureConfigurator<T> IFixtureConfigurator<T>.Invoke(Expression<Action<T>> expr)
        {
            _fixture ??= InstantiateFixture();

            ExpressionValidator.ValidateMethodExpression(expr);
            _expressionResolver.ResolveMethodParent(_fixture, expr, _context);

            var action = expr.Compile();
            action.Invoke(_fixture);

            return this;
        }

        IFixtureConfigurator<T> IFixtureConfigurator<T>.InvokePrivate(string methodName, params object?[] arguments)
        {
            _fixture ??= InstantiateFixture();

            InvokePrivateInternal(typeof(T), _fixture, methodName, arguments);

            return this;
        }

        IFixtureConfigurator<T> IFixtureConfigurator<T>.InvokePrivate<TProp>(Expression<Func<T, TProp>> expr, string methodName, params object?[] arguments)
        {
            _fixture ??= InstantiateFixture();

            ExpressionValidator.ValidateExpression(expr);

            var (instance, dataMember) = _expressionResolver.ResolveDataMemberInstance(_fixture, expr, _context);
            var dataMemberType = dataMember.DataMemberType;

            InvokePrivateInternal(dataMemberType, instance, methodName, arguments);

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
            if (!_context.OptionsFor(typeof(T)).AllowImplicitConstruction)
                throw new InvalidOperationException($"Skipping instantiation step is not allowed. " +
                    $"Please use UseAutoConstructor, UseConstructor, or CreateUninitialized before calling any configuration methods. " +
                    $"Explicit instantiation can be allowed via AllowImplicitConstruction option.");

            var request = new FixtureRequest(typeof(T));

            return (T)_context.InstantiateWithStrategy(request, _context.OptionsFor(typeof(T)).DefaultInstantiationMethod, _context.OptionsFor(typeof(T)).DefaultInitializeMembers);
        }

        private static IFixtureContext InitializeContext()
        {
            return FixtureContextFactory.CreateLazyContext();
        }

        private static ExpressionResolver InitializeExpressionResolver()
        {
            return new ExpressionResolver(typeof(T));
        }
    }
}
