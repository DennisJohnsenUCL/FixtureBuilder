using FixtureBuilder.TypeLinks;
using FixtureBuilder.ValueConverters;

namespace FixtureBuilder.FixtureContexts
{
    internal class LazyContextResolver : IContextResolver
    {
        private readonly Lazy<IValueConverter> _converter;
        private readonly Lazy<ITypeLink> _typeLink;

        public LazyContextResolver(Func<IValueConverter> converter, Func<ITypeLink> typeLink)
        {
            ArgumentNullException.ThrowIfNull(converter);
            ArgumentNullException.ThrowIfNull(typeLink);

            _converter = new Lazy<IValueConverter>(converter);
            _typeLink = new Lazy<ITypeLink>(typeLink);
        }

        public IValueConverter GetConverter()
            => _converter.Value;

        public ITypeLink GetTypeLink()
            => _typeLink.Value;
    }
}
