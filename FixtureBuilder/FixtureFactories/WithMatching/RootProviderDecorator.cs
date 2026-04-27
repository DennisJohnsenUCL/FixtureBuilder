using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.FixtureFactories.WithMatching
{
    internal class RootProviderDecorator : IValueProvider
    {
        private readonly IValueProvider _inner;
        private readonly Type _rootType;

        public RootProviderDecorator(IValueProvider provider, Type rootType)
        {
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(rootType);
            _inner = provider;
            _rootType = rootType;
        }

        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.RootType != _rootType)
                return new NoResult();

            return _inner.ResolveValue(request, context);
        }
    }
}
