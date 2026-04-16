using FixtureBuilder.Assignment.ValueProviders.Providers;

namespace FixtureBuilder.Assignment.ValueProviders
{
    internal class ValueProviderFactory
    {
        public static ICompositeValueProvider CreateDefaultValueProvider()
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
