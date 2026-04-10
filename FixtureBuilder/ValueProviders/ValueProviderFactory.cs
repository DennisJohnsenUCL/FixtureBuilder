using FixtureBuilder.ValueProviders.Providers;

namespace FixtureBuilder.ValueProviders
{
    internal class ValueProviderFactory
    {
        public static IValueProvider CreateDefaultValueProvider()
        {
            var provider = new CompositeValueProvider([
                new DefaultParameterProvider(),
                new NullableParameterProvider(),
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
