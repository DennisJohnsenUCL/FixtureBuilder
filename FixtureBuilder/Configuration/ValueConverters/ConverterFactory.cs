using FixtureBuilder.Configuration.ValueConverters.Casters;
using FixtureBuilder.Configuration.ValueConverters.CollectionConverters;
using FixtureBuilder.Configuration.ValueConverters.Converters;
using FixtureBuilder.Configuration.ValueConverters.DictionaryConverters;

namespace FixtureBuilder.Configuration.ValueConverters
{
    internal class ConverterFactory
    {
        public static ICompositeConverter CreateDefaultConverter()
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

            var composite = new CompositeConverter(converters, new EnumerableElementCaster(), new DictionaryElementCaster());

            return composite;
        }
    }
}
