using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters.Decorators
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
            target = context.UnwrapAndLink(target);

            var result = _inner.Convert(target, value, context);
            if (result != null) return result;

            return null;
        }
    }
}
