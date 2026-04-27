using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.FixtureFactories
{
    internal class CustomProviderAdapter : IValueProvider
    {
        private readonly ICustomProvider _provider;

        public CustomProviderAdapter(ICustomProvider provider)
        {
            ArgumentNullException.ThrowIfNull(provider);
            _provider = provider;
        }

        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            return _provider.ResolveValue(request);
        }
    }
}
