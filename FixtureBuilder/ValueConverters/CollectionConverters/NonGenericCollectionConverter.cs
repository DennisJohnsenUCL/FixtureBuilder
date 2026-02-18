using FixtureBuilder.FixtureContexts;
using FixtureBuilder.Helpers;
using System.Collections;

namespace FixtureBuilder.ValueConverters.CollectionConverters
{
    internal class NonGenericCollectionConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(ArrayList), typeof(Stack), typeof(Queue)];

        public object? Convert(Type target, object value, IFixtureContext context)
        {
            if (_types.Contains(target)
                && value is IEnumerable enumerable)
            {
                if (value is not ICollection) value = CastToArray(enumerable);

                var collection = InstantiationHelpers.UseConstructor(target, value);
                return collection;
            }

            return null;
        }

        private static Array CastToArray(IEnumerable value)
        {
            var valuesList = value.Cast<object>().ToList();
            var array = Array.CreateInstance(typeof(object), valuesList.Count);
            for (int i = 0; i < valuesList.Count; i++)
            {
                array.SetValue(valuesList[i], i);
            }
            return array;
        }
    }
}
