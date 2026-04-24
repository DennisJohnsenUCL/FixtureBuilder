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

        public object? Convert(Type target, object value, IFixtureContext context)
        {
            if (value is IEnumerable enumerable
                && target.GetEnumerableElementType() is Type targetElementType
                && value.GetType().GetEnumerableElementType() != targetElementType
                && target != typeof(string)
                && !target.IsArray
                && !target.IsDictionary())
            {
                var typedList = CastElements(enumerable, targetElementType, context);

                return _inner.Convert(target, typedList, context);
            }
            return _inner.Convert(target, value, context);
        }

        private static IEnumerable CastElements(IEnumerable values, Type elementType, IFixtureContext context)
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
                    var converted = context.Converter.Root.Convert(elementType, item, context);
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
