using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.ValueConverters.Decorators
{
    internal class TypeLinkingConverter : IValueConverter
    {
        private readonly IValueConverter _inner;

        public TypeLinkingConverter(IValueConverter inner)
        {
            ArgumentNullException.ThrowIfNull(inner);

            _inner = inner;
        }

        public object? Convert(Type target, object value, IFixtureContext context)
        {
            var nullableType = Nullable.GetUnderlyingType(target);
            if (nullableType != null) target = nullableType;

            var link = context.Link(target);
            if (link != null) target = link;

            var result = _inner.Convert(target, value, context);
            if (result != null) return result;

            return null;
        }
    }
}
