using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters
{
    internal class CompositeConverter : IValueConverter
    {
        private readonly IEnumerable<IValueConverter> _converters;

        public CompositeConverter(IEnumerable<IValueConverter> converters)
        {
            ArgumentNullException.ThrowIfNull(converters);

            _converters = converters;
        }

        public object? Convert(Type target, object value, IFixtureContext context)
        {
            foreach (var converter in _converters)
            {
                var result = converter.Convert(target, value, context);
                if (result is not NoResult) return result;
            }
            return new NoResult();
        }
    }
}
