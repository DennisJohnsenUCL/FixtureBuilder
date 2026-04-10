using System.Collections.ObjectModel;
using FixtureBuilder.Extensions;
using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.ValueConverters.DictionaryConverters
{
    internal class SpecializedGenericDictionaryConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(ReadOnlyDictionary<,>), typeof(SortedDictionary<,>),
            typeof(SortedList<,>)];

        public object? Convert(Type target, object value, IFixtureContext context)
        {
            if (_types.Contains(target.GetGenericTypeDefinitionOrDefault())
                && value.GetType().GetEnumerableElementType() == target.GetEnumerableElementType())
            {
                if (!value.GetType().Implements(typeof(IDictionary<,>)))
                {
                    var (keyType, valueType) = target.GetDictionaryEnumerableTypes();
                    var intermediateType = typeof(Dictionary<,>).MakeGenericType(keyType!, valueType!);
                    value = Activator.CreateInstance(intermediateType, value)!;
                }

                return Activator.CreateInstance(target, value);
            }
            return new NoResult();
        }
    }
}
