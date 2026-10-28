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

        public object? Convert(TypeRequest request, object value, IFixtureContext context)
        {
            var type = context.UnwrapAndLink(request.Type, request.RootType);
            request = new TypeRequest(type, request.RootType);

            var result = _inner.Convert(request, value, context);
            if (result != null) return result;

            return null;
        }
    }
}
