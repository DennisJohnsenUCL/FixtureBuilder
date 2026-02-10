namespace FixtureBuilder.ValueConverters.Decorators
{
    internal class ValidatingConverter : IValueConverter
    {
        private readonly IValueConverter _inner;

        public ValidatingConverter(IValueConverter inner)
        {
            ArgumentNullException.ThrowIfNull(inner);

            _inner = inner;
        }

        public object? Convert(Type target, object value)
        {
            ArgumentNullException.ThrowIfNull(target);
            if (value == null) return null;
            if (value.GetType().IsAssignableTo(target)) return value;

            var result = _inner.Convert(target, value);
            if (result != null && result.GetType().IsAssignableTo(target)) return result;

            throw new InvalidOperationException($"Failed to convert {value.GetType().Name} to {target.Name}.");
        }
    }
}
