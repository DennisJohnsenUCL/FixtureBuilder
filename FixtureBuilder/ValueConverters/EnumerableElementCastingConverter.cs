using FixtureBuilder.Extensions;
using System.Collections;

namespace FixtureBuilder.ValueConverters
{
    internal class EnumerableElementCastingConverter : IValueConverter
    {
        private readonly IValueConverter _inner;

        public EnumerableElementCastingConverter(IValueConverter inner)
        {
            _inner = inner;
        }

        public object? Convert(Type target, object value)
        {
            if (value is IEnumerable enumerable && target.Implements(typeof(IEnumerable<>)))
            {
                var sourceType = enumerable.GetType();
                var sourceElementType = sourceType.IsGenericType ? sourceType.GenericTypeArguments[0] : typeof(object);

                var targetElementType = target.GetGenericArguments()[0];

                var typedList = sourceElementType == targetElementType
                    ? enumerable
                    : CastElements(enumerable, targetElementType);

                return _inner.Convert(target, typedList);
            }
            return _inner.Convert(target, value);
        }

        private static IEnumerable CastElements(IEnumerable values, Type elementType)
        {
            var typedList = typeof(Enumerable)
                .GetMethod("Cast")!
                .MakeGenericMethod(elementType)
                .Invoke(null, [values])!;

            return (IEnumerable)typedList;
        }
    }
}
