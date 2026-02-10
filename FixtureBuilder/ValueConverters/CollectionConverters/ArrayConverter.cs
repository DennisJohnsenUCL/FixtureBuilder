using System.Collections;

namespace FixtureBuilder.ValueConverters.CollectionConverters
{
    internal class ArrayConverter : IValueConverter
    {
        public object? Convert(Type target, object value)
        {
            if (target.IsArray
                && value is IEnumerable enumerable)
            {
                var valuesList = enumerable.Cast<object>().ToList();
                var elementType = target.GetElementType()!;
                var array = Array.CreateInstance(elementType, valuesList.Count);
                for (int i = 0; i < valuesList.Count; i++)
                {
                    array.SetValue(valuesList[i], i);
                }
                return array;
            }

            return null;
        }
    }
}
