using FixtureBuilder.Helpers;
using System.Collections;

namespace FixtureBuilder.ValueConverters
{
    internal class MutableNonGenericCollectionConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(ArrayList), typeof(Stack), typeof(Queue)];

        public object? Convert(Type target, object value)
        {
            ArgumentNullException.ThrowIfNull(target);
            if (value == null) return null;
            if (value.GetType() == target) return value;

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
