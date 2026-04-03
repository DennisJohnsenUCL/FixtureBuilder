using FixtureBuilder.FixtureProviders.Providers;

namespace FixtureBuilder.FixtureProviders.FixtureProviderBuilders
{
    /// <summary>
    /// Creates the default <see cref="IFixtureProvider"/> pipeline, composing the standard set of
    /// type-specific providers into a <see cref="CompositeFixtureProvider"/>.
    /// </summary>
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
