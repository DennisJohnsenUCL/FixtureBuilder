
using FixtureBuilder.ValueProviders.Providers;

namespace FixtureBuilder.ValueProviders.ValueProviderBuilders
{
    /// <summary>
    /// Creates the default <see cref="IValueProvider"/> pipeline, composing the standard set of
    /// type-specific providers into a <see cref="CompositeValueProvider"/>.
    /// </summary>
    internal class ValueProviderFactory : IValueProviderFactory
    {
        public IValueProvider CreateDefaultValueProvider()
        {
            var provider = new CompositeValueProvider([
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
