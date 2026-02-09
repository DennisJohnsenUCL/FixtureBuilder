namespace FixtureBuilder.ValueConverters
{
    internal class ThrowingConverter : IValueConverter
    {
        private readonly IValueConverter _inner;

        public ThrowingConverter(IValueConverter inner)
        {
            _inner = inner;
        }

        public object? Convert(Type target, object value)
        {
            ArgumentNullException.ThrowIfNull(target);
            if (value == null) return null;
            if (value.GetType() == target) return value;

            var result = _inner.Convert(target, value);
            if (result != null) return result;

            throw new InvalidOperationException($"Failed to convert {value.GetType().Name} to {target.Name}.");
        }
    }
}
