using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters.Decorators
{
    internal class ValidatingConverter : IValueConverter
    {
        private readonly IValueConverter _inner;

        public ValidatingConverter(IValueConverter inner)
        {
            ArgumentNullException.ThrowIfNull(inner);

            _inner = inner;
        }

        public object? Convert(FixtureRequest request, object value, IFixtureContext context)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(context);
            if (value == null) return null;
            if (value.GetType().IsAssignableTo(request.Type)) return value;

            var result = _inner.Convert(request, value, context);
            if (result is not NoResult && result != null && !result.GetType().IsAssignableTo(request.Type)) return new NoResult();
            return result;
        }
    }
}
