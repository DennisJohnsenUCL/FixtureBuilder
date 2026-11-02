using System.Collections;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.Casters
{
    internal class EnumerableElementCaster : IEnumerableElementCaster
    {
        public object Cast(TypeRequest request, object value, IFixtureContext context)
        {
            var target = request.Type;
            if (value is IEnumerable enumerable
                && target.GetEnumerableElementType() is { } targetElementType
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
                list.Add(ElementConverter.Convert(item, elementType, rootType, context));
            }

            return list;
        }
    }
}
