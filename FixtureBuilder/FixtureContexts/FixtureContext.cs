using FixtureBuilder.UninitializedProviders;

namespace FixtureBuilder.FixtureContexts
{
    internal class FixtureContext : IFixtureContext
    {
        private readonly IContextResolver _resolver;

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
    }
}
