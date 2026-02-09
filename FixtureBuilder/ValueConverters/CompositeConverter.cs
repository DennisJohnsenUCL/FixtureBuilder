namespace FixtureBuilder.ValueConverters
{
    internal class CompositeConverter : IValueConverter
    {
        private readonly IEnumerable<IValueConverter> _converters;

        public CompositeConverter(IEnumerable<IValueConverter> converters) => _converters = converters;

        public object? Convert(Type target, object value)
        {
            ArgumentNullException.ThrowIfNull(target);
            if (value == null) return null;
            if (value.GetType() == target) return value;

            foreach (var converter in _converters)
            {
                var result = converter.Convert(target, value);
                if (result != null) return result;
            }
            return null;
        }
    }
}
