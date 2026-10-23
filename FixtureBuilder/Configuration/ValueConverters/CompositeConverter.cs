using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters
{
    /// <summary>
    /// An <see cref="IValueConverter"/> that iterates through a sequence of
    /// <see cref="IValueConverter"/> instances, returning the first result that is not <see cref="NoResult"/>.
    /// Root-scoped converters are always tried before global and built-in ones, regardless of registration order.
    /// Returns <see cref="NoResult"/> if no converter handles the request.
    /// </summary>
    internal class CompositeConverter : ICompositeConverter
    {
        private IEnumerable<IValueConverter> _converters;
        private IEnumerable<IValueConverter> _rootConverters;

        public CompositeConverter(IEnumerable<IValueConverter> converters)
        {
            ArgumentNullException.ThrowIfNull(converters);

            _converters = converters;
            _rootConverters = [];
        }

        public object? Convert(FixtureRequest request, object value, IFixtureContext context)
        {
            foreach (var converter in _rootConverters.Concat(_converters))
            {
                var result = converter.Convert(request, value, context);
                if (result is not NoResult) return result;
            }
            return new NoResult();
        }

        public void AddConverter(IValueConverter converter)
        {
            _converters = _converters.Prepend(converter);
        }

        public void AddRootConverter(IValueConverter rootConverter)
        {
            _rootConverters = _rootConverters.Prepend(rootConverter);
        }
    }
}
