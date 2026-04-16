using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core.FixtureContexts.ContextResolvers;
using FixtureBuilder.Creation;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Core.FixtureContexts
{
    /// <summary>
    /// Context for fixture object graphs. Delegates all operations
    /// to components resolved through an <see cref="IContextResolver"/>.
    /// </summary>
    internal class FixtureContext : IFixtureContext
    {
        private readonly IContextResolver _resolver;

        public FixtureOptions Options { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureContext"/> class.
        /// </summary>
        /// <param name="resolver">The resolver used to obtain converter, type link, and uninitialized provider components.</param>
        /// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
        public FixtureContext(IContextResolver resolver, FixtureOptions options)
        {
            ArgumentNullException.ThrowIfNull(resolver);
            ArgumentNullException.ThrowIfNull(options);

            _resolver = resolver;
            Options = options;
        }

        public object? Convert(Type target, object value, IFixtureContext context)
        {
            return _resolver.Converter.Root.Convert(target, value, context);
        }

        public void AddConverter(IValueConverter converter)
        {
            _resolver.Converter.Composite.AddConverter(converter);
        }

        public Type? Link(Type target)
        {
            return _resolver.TypeLink.Link(target);
        }

        public void AddTypeLink(ITypeLink link)
        {
            _resolver.TypeLink.AddTypeLink(link);
        }

        public object? ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context, RecursiveResolveContext? recursiveResolveContext = null)
        {
            return _resolver.UninitializedProvider.ResolveUninitialized(request, initializeMembers, context, recursiveResolveContext);
        }

        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            return _resolver.ValueProvider.ResolveValue(request, context);
        }

        public void AddProvider(IValueProvider provider)
        {
            _resolver.ValueProvider.AddProvider(provider);
        }

        public object AutoResolve(FixtureRequest request, IFixtureContext context, RecursiveResolveContext? recursiveResolveContext = null)
        {
            return _resolver.AutoConstructingProvider.AutoResolve(request, context, recursiveResolveContext);
        }

        public void SetOptions(Action<FixtureOptions> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            action.Invoke(Options);
        }

        public Type UnwrapAndLink(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            type = _resolver.TypeLink.Link(type) ?? type;
            return type;
        }

        public object? ProvideWithStrategy(FixtureRequest request, InstantiationMethod instantiationMethod, InitializeMembers initializeMembers)
        {
            request.Type = UnwrapAndLink(request.Type);
            var result = ResolveValue(request, this);
            if (result is not NoResult) return result;

            return InstantiateWithStrategy(request, instantiationMethod, initializeMembers);
        }

        public object InstantiateWithStrategy(FixtureRequest request, InstantiationMethod instantiationMethod, InitializeMembers initializeMembers)
        {
            return instantiationMethod switch
            {
                InstantiationMethod.UseAutoConstructor => AutoResolve(request, this),

                InstantiationMethod.UseDefaultConstructor => new ConstructingProvider().ResolveWithArguments(request),

                InstantiationMethod.CreateUninitialized => ResolveUninitialized(request, initializeMembers, this)
                    ?? throw new InvalidOperationException($"Failed to instantiate {request.Type.Name} uninitialized."),

                _ => throw new InvalidOperationException($"Invalid instantiation method: {instantiationMethod}.")
            };
        }
    }
}
