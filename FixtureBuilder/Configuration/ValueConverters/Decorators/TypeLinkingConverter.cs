using FixtureBuilder.Core;
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

        public object? Convert(FixtureRequest request, object value, IFixtureContext context)
        {
            request.Type = context.UnwrapAndLink(request);

            var result = _inner.Convert(request, value, context);
            if (result != null) return result;

            return null;
        }
    }
}
