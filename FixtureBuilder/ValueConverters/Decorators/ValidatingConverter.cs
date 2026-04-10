using FixtureBuilder.FixtureContexts;

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

        public object? Convert(Type target, object value, IFixtureContext context)
        {
            ArgumentNullException.ThrowIfNull(target);
            ArgumentNullException.ThrowIfNull(context);
            if (value == null) return null;
            if (value.GetType().IsAssignableTo(target)) return value;

            var result = _inner.Convert(target, value, context);
            if (result is not NoResult && result != null && !result.GetType().IsAssignableTo(target)) return new NoResult();
            return result;
        }
    }
}
