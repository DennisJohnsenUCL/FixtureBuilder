using System.Reflection;
using FixtureBuilder.ConstructingProviders;
using FixtureBuilder.UninitializedProviders;

namespace FixtureBuilder.FixtureContexts
{
    /// <summary>
    /// Context for fixture object graphs. Delegates all operations
    /// to components resolved through an <see cref="IContextResolver"/>.
    /// </summary>
    internal class FixtureContext : IFixtureContext
    {
        private readonly IContextResolver _resolver;

        public FixtureOptions Options { get; private set; }

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
            return _resolver.Converter.Convert(target, value, context);
        }

        public Type? Link(Type target)
        {
            return _resolver.TypeLink.Link(target);
        }

        public object? ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context)
        {
            return _resolver.UninitializedProvider.ResolveUninitialized(request, initializeMembers, context);
        }

        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            return _resolver.ValueProvider.ResolveValue(request, context);
        }

        public object? ResolveParameterValue(ParameterInfo paramInfo, IFixtureContext context)
        {
            return _resolver.ParameterProvider.ResolveParameterValue(paramInfo, context);
        }

        public object AutoResolve(FixtureRequest request, IFixtureContext context)
        {
            return _resolver.AutoConstructingProvider.AutoResolve(request, context);
        }

        public void SetOptions(FixtureOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
            Options = options;
        }

        public void SetOptions(Action<FixtureOptions> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            action.Invoke(Options);
        }

        public object InstantiateWithStrategy(FixtureRequest request, InstantiationMethod instantiationMethod, InitializeMembers initializeMembers)
        {
            return instantiationMethod switch
            {
                InstantiationMethod.UseAutoConstructor => AutoResolve(request, this),

                InstantiationMethod.UseDefaultConstructor => new ConstructingProvider().ResolveWithArguments(request),

                InstantiationMethod.CreateUninitialized => ResolveUninitialized(request, initializeMembers, this)
                    ?? throw new InvalidOperationException($"Failed to intantiate {request.Type.Name} uninitialized."),

                _ => throw new InvalidOperationException($"Failed to intantiate {request.Type.Name} uninitialized.")
            };
        }
    }
}
