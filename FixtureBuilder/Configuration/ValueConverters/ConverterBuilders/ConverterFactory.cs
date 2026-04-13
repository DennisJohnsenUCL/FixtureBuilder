using FixtureBuilder.Configuration.ValueConverters;

namespace FixtureBuilder.Configuration.ValueConverters.ConverterBuilders
{
    internal class ConverterFactory
    {
        public static IValueConverter CreateDefaultConverter()
        {
            var converter = new ConverterBuilder()
                .WithStrategies()
                    .AddEnumerableStrategies()
                    .AddDictionaryStrategies()
                    .And()
                .WithDictionaryElementCasting()
                .WithEnumerableElementCasting()
                .WithTypeLinking()
                .WithValidation()
                .Build();

            return converter;
        }
    }
}
