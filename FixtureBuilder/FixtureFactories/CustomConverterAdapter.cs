using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.FixtureFactories
{
    internal class CustomConverterAdapter : IValueConverter
    {
        private readonly ICustomConverter _converter;
        private readonly Type? _rootType;

        public CustomConverterAdapter(ICustomConverter converter)
        {
            ArgumentNullException.ThrowIfNull(converter);
            _converter = converter;
        }

        public CustomConverterAdapter(ICustomConverter converter, Type rootType) : this(converter)
        {
            ArgumentNullException.ThrowIfNull(rootType);
            _rootType = rootType;
        }

        public object? Convert(FixtureRequest request, object value, IFixtureContext context)
        {
            if (_rootType != null && request.RootType != _rootType)
                return new NoResult();

            return _converter.Convert(request.Type, value);
        }
    }
}
