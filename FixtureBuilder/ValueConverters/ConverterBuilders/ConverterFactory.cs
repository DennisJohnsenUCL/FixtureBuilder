namespace FixtureBuilder.ValueConverters.ConverterBuilders
{
    internal interface IConverterFactory
    {
        IValueConverter CreateDefaultConverter();
    }

    internal class ConverterFactory : IConverterFactory
    {
        public ConverterFactory() { }

        public IValueConverter CreateDefaultConverter()
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
