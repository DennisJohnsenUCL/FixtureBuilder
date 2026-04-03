using FixtureBuilder.FixtureProviders.Providers;

namespace FixtureBuilder.FixtureProviders.FixtureProviderBuilders
{
    internal class FixtureProviderFactory : IFixtureProviderFactory
    {
        public IFixtureProvider CreateDefaultFixtureProvider()
        {
            var provider = new CompositeFixtureProvider([
                new StringProvider(),
                new EnumProvider(),
                new DateTimeProvider(),
                new TimeSpanProvider(),
                new PrimitiveNumberProvider(),
                new ArrayProvider(),
                new ConstructibleEnumerableProvider(),
                new ImmutableFrozenEnumerableProvider()]);

            return provider;
        }
    }
}
