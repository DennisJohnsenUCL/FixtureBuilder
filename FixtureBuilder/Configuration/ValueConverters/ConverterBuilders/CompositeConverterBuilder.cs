using FixtureBuilder.Configuration.ValueConverters.CollectionConverters;
using FixtureBuilder.Configuration.ValueConverters.DictionaryConverters;

namespace FixtureBuilder.Configuration.ValueConverters.ConverterBuilders
{
    internal class CompositeConverterBuilder
    {
        private readonly ConverterBuilder _builder;
        private IEnumerable<IValueConverter> _converters = [];

        public CompositeConverterBuilder(ConverterBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            _builder = builder;
        }

        public CompositeConverterBuilder AddDictionaryStrategies()
        {
            _converters = _converters.Concat([new MutableGenericDictionaryConverter(),
                new ImmutableDictionaryConverter(),
                new FrozenDictionaryConverter(),
                new SpecializedGenericDictionaryConverter(),
                new NonGenericDictionaryConverter()]);

            return this;
        }

        public CompositeConverterBuilder AddEnumerableStrategies()
        {
            _converters = _converters.Concat([new MutableGenericCollectionConverter(),
                new ImmutableCollectionConverter(),
                new FrozenSetConverter(),
                new ArrayConverter(),
                new NonGenericCollectionConverter(),
                new BlockingCollectionConverter()]);

            return this;
        }

        public ConverterBuilder And()
        {
            _builder.WithComposite(new CompositeConverter(_converters));
            return _builder;
        }
    }
}
