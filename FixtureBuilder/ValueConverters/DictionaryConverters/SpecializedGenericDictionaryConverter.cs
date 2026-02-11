using FixtureBuilder.Extensions;
using FixtureBuilder.Helpers;
using System.Collections.ObjectModel;

namespace FixtureBuilder.ValueConverters.DictionaryConverters
{
    internal class SpecializedGenericDictionaryConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(ReadOnlyDictionary<,>), typeof(SortedDictionary<,>),
            typeof(SortedList<,>)];

        public object? Convert(Type target, object value)
        {
            if (_types.Contains(target.GetGenericTypeDefinitionOrDefault())
                && value.GetType().GetEnumerableElementType() == target.GetEnumerableElementType())
            {
                if (!value.GetType().Implements(typeof(IDictionary<,>)))
                {
                    var (keyType, valueType) = target.GetDictionaryEnumerableTypes();
                    var intermediateType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                    value = InstantiationHelpers.UseConstructor(intermediateType, value)!;
                }

                return InstantiationHelpers.UseConstructor(target, value);
            }
            return null;
        }
    }
}
