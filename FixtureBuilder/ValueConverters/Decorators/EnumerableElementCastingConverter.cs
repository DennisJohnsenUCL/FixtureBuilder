using FixtureBuilder.Extensions;
using System.Collections;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace FixtureBuilder.ValueConverters.Decorators
{
    internal class EnumerableElementCastingConverter : IValueConverter
    {
        private readonly IValueConverter _inner;

        public EnumerableElementCastingConverter(IValueConverter inner)
        {
            ArgumentNullException.ThrowIfNull(inner);

            _inner = inner;
        }

        public object? Convert(Type target, object value)
        {
            if (value is IEnumerable enumerable
                && target != typeof(string)
                && target.Implements(typeof(IEnumerable<>))
                && !target.IsArray)
            {
                var sourceType = value.GetType();
                var sourceElementType = sourceType.IsGenericType ? sourceType.GenericTypeArguments[0] : typeof(object);

                var targetElementType = target.GetEnumerableElementType()!;

                var typedList = sourceElementType == targetElementType
                    ? enumerable
                    : CastElements(enumerable, targetElementType);

                return _inner.Convert(target, typedList);
            }
            return _inner.Convert(target, value);
        }

        private static IEnumerable CastElements(IEnumerable values, Type elementType)
        {
            var typedElements = typeof(Enumerable)
                .GetMethod("Cast")!
                .MakeGenericMethod(elementType)
                .Invoke(null, [values])!;

            try
            {
                var typedList = typeof(Enumerable)
                    .GetMethod("ToList")!
                    .MakeGenericMethod(elementType)
                    .Invoke(null, [typedElements])!;

                return (IEnumerable)typedList;
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
    }
}
