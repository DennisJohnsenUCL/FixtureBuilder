using FixtureBuilder.TypeLinkers;

namespace FixtureBuilder.ValueConverters
{
    internal class TypeLinkingConverter : IValueConverter
    {
        private readonly IValueConverter _inner;
        private readonly ITypeLink _typeLink;

        public TypeLinkingConverter(IValueConverter inner, ITypeLink typeLink)
        {
            _inner = inner;
            _typeLink = typeLink;
        }

        public object? Convert(Type target, object value)
        {
            var link = _typeLink.Link(target);
            if (link != null) target = link;

            var result = _inner.Convert(target, value);
            if (result != null) return result;

            return null;
        }
    }
}
