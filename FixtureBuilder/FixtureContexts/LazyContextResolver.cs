using FixtureBuilder.TypeLinks;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.ValueConverters;

namespace FixtureBuilder.FixtureContexts
{
    internal class LazyContextResolver : IContextResolver
    {
        private readonly Lazy<IValueConverter> _converter;
        private readonly Lazy<ITypeLink> _typeLink;
        private readonly Lazy<IFixtureUninitializedProvider> _uninitializedProvider;

        public LazyContextResolver(Func<IValueConverter> converter,
            Func<ITypeLink> typeLink,
            Func<IFixtureUninitializedProvider> uninitializedProvider)
        {
            ArgumentNullException.ThrowIfNull(converter);
            ArgumentNullException.ThrowIfNull(typeLink);
            ArgumentNullException.ThrowIfNull(uninitializedProvider);

            _converter = new Lazy<IValueConverter>(converter);
            _typeLink = new Lazy<ITypeLink>(typeLink);
            _uninitializedProvider = new Lazy<IFixtureUninitializedProvider>(uninitializedProvider);
        }

        public IValueConverter GetConverter()
            => _converter.Value;

        public ITypeLink GetTypeLink()
            => _typeLink.Value;

        public IFixtureUninitializedProvider GetUninitializedProvider()
            => _uninitializedProvider.Value;
    }
}
