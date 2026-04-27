using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.FixtureFactories.RootProviderBuilders
{
    internal class RootConverterDecorator : IValueConverter
    {
        private readonly IValueConverter _inner;
        private readonly Type _rootType;

        public RootConverterDecorator(IValueConverter converter, Type rootType)
        {
            ArgumentNullException.ThrowIfNull(converter);
            ArgumentNullException.ThrowIfNull(rootType);
            _inner = converter;
            _rootType = rootType;
        }

        public object? Convert(FixtureRequest request, object value, IFixtureContext context)
        {
            if (request.RootType != _rootType)
                return new NoResult();

            return _inner.Convert(request, value, context);
        }
    }
}
