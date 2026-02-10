using FixtureBuilder.Extensions;
using FixtureBuilder.Helpers;
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
                && target.GetEnumerableElementType() is Type targetElementType
                && value.GetType().GetEnumerableElementType() != targetElementType
                && target != typeof(string)
                && !target.IsArray
                && !CollectionHelpers.IsDictionary(target))
            {
                var typedList = CastElements(enumerable, targetElementType);

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
