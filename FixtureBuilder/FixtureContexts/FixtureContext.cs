using System.Reflection;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureContext"/> class.
        /// </summary>
        /// <param name="resolver">The resolver used to obtain converter, type link, and uninitialized provider components.</param>
        /// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
        public FixtureContext(IContextResolver resolver)
        {
            ArgumentNullException.ThrowIfNull(resolver);

            _resolver = resolver;
        }

        public object? Convert(Type target, object value, IFixtureContext context)
        {
            return _resolver.GetConverter().Convert(target, value, context);
        }

        public Type? Link(Type target)
        {
            return _resolver.GetTypeLink().Link(target);
        }

        public object? ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context)
        {
            return _resolver.GetUninitializedProvider().ResolveUninitialized(request, initializeMembers, context);
        }

        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            return _resolver.GetValueProvider().ResolveValue(request, context);
        }

        public object? ResolveParameterValue(ParameterInfo paramInfo, IFixtureContext context)
        {
            return _resolver.GetParameterProvider().ResolveParameterValue(paramInfo, context);
        }

        public object AutoResolve(FixtureRequest request, IFixtureContext context)
        {
            return _resolver.GetAutoConstructingProvider().AutoResolve(request, context);
        }
    }
}
