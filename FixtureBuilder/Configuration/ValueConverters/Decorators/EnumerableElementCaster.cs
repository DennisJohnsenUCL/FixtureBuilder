using System.Collections;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.Decorators
{
    internal class EnumerableElementCaster : IEnumerableElementCaster
    {
        public object Cast(TypeRequest request, object value, IFixtureContext context)
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

                return typedList;
            }
            return value;
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
                    var request = new TypeRequest(elementType, rootType);
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
