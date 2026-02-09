using FixtureBuilder.Extensions;
using System.Collections;

namespace FixtureBuilder.ValueConverters
{
    internal class ArrayConverter : IValueConverter
    {
        public object? Convert(Type target, object value)
        {
            ArgumentNullException.ThrowIfNull(target);
            if (value == null) return null;

            var sourceType = value.GetType();
            if (sourceType == target) return value;

            if (target.IsArray
                && sourceType.Implements(typeof(IEnumerable<>))
                && sourceType.IsGenericType
                && sourceType.GenericTypeArguments[0] == target.GetElementType())
            {
                var valuesList = ((IEnumerable)value).Cast<object>().ToList();
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
