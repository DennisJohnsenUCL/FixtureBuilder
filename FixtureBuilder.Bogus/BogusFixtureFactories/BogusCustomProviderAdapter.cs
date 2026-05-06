using Bogus;
using FixtureBuilder.Core;

namespace FixtureBuilder.Bogus.BogusFixtureFactories
{
    internal class BogusCustomProviderAdapter : ICustomProvider
    {
        private readonly IBogusCustomProvider _provider;
        private readonly Faker _faker;

        public BogusCustomProviderAdapter(IBogusCustomProvider provider, Faker faker)
        {
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(faker);
            _provider = provider;
            _faker = faker;
        }

        public object? ResolveValue(FixtureRequest request)
        {
            return _provider.ResolveValue(request, _faker);
        }
    }
}
