using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.FixtureFactories
{
    internal class CustomConverterAdapter : IValueConverter
    {
        private readonly ICustomConverter _converter;

        public CustomConverterAdapter(ICustomConverter converter)
        {
            ArgumentNullException.ThrowIfNull(converter);
            _converter = converter;
        }

        public object? Convert(Type target, object value, IFixtureContext context)
        {
            return _converter.Convert(target, value);
        }
    }
}
