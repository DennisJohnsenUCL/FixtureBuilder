using System.Collections;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.Decorators
{
    internal class EnumerableElementCastingConverter : IValueConverter
    {
        private readonly IValueConverter _inner;

        public EnumerableElementCastingConverter(IValueConverter inner)
        {
            ArgumentNullException.ThrowIfNull(inner);

            _inner = inner;
        }

        public object? Convert(FixtureRequest request, object value, IFixtureContext context)
        {
            var target = request.Type;
            if (value is IEnumerable enumerable
                && target.GetEnumerableElementType() is Type targetElementType
                && value.GetType().GetEnumerableElementType() != targetElementType
                && target != typeof(string)
                && !target.IsArray
                && !target.IsDictionary())
            {
                var typedList = CastElements(enumerable, targetElementType, request.RootType, context);

                return _inner.Convert(request, typedList, context);
            }
            return _inner.Convert(request, value, context);
        }

        private static IEnumerable CastElements(IEnumerable values, Type elementType, Type rootType, IFixtureContext context)
        {
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType)!;

            foreach (var item in values)
            {
                if (item == null)
                {
                    list.Add(null);
                }
                else if (elementType.IsAssignableFrom(item.GetType()))
                {
                    list.Add(item);
                }
                else
                {
                    var request = new FixtureRequest(elementType, rootType);
                    var converted = context.Converter.Root.Convert(request, item, context);
                    if (converted is NoResult)
                        throw new InvalidCastException(
                            $"Cannot convert element of type {item.GetType().Name} to {elementType.Name}.");
                    list.Add(converted);
                }
            }

            return list;
        }
    }
}
