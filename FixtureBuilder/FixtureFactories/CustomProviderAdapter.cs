using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.FixtureFactories
{
    internal class CustomProviderAdapter : IValueProvider
    {
        private readonly ICustomProvider _provider;
        private readonly Type? _rootType;

        public CustomProviderAdapter(ICustomProvider provider)
        {
            ArgumentNullException.ThrowIfNull(provider);
            _provider = provider;
        }

        public CustomProviderAdapter(ICustomProvider provider, Type rootType) : this(provider)
        {
            ArgumentNullException.ThrowIfNull(rootType);
            _rootType = rootType;
        }

        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (_rootType != null && request.RootType != _rootType)
                return new NoResult();

            return _provider.ResolveValue(request);
        }
    }
}
