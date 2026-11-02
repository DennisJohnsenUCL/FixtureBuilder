using FixtureBuilder.Configuration.ValueConverters.CollectionConverters;
using FixtureBuilder.Configuration.ValueConverters.Converters;
using FixtureBuilder.Configuration.ValueConverters.Decorators;
using FixtureBuilder.Configuration.ValueConverters.DictionaryConverters;

namespace FixtureBuilder.Configuration.ValueConverters
{
    internal class ConverterFactory
    {
        public static ConverterGraph CreateDefaultConverter()
        {
            var converters = new List<IValueConverter>([
                new MutableGenericCollectionConverter(),
                new CollectionConverter(),
                new ImmutableCollectionConverter(),
                new FrozenSetConverter(),
                new ArrayConverter(),
                new NonGenericCollectionConverter(),
                new BlockingCollectionConverter(),
                new MutableGenericDictionaryConverter(),
                new ImmutableDictionaryConverter(),
                new FrozenDictionaryConverter(),
                new SpecializedGenericDictionaryConverter(),
                new NonGenericDictionaryConverter(),
                new ImplicitConverter()]);

            var composite = new CompositeConverter(converters, new EnumerableElementCaster());

            var converter = new ValidatingConverter(
                new TypeLinkingConverter(
                        new DictionaryElementCastingConverter(composite))) as IValueConverter;

            var graph = new ConverterGraph(converter, composite);

            return graph;
        }
    }
}
