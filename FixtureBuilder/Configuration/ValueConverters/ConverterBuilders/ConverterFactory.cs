namespace FixtureBuilder.Configuration.ValueConverters.ConverterBuilders
{
    internal class ConverterFactory
    {
        public static ConverterGraph CreateDefaultConverter()
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
