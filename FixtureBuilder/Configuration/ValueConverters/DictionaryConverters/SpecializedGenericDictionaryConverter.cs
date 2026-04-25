using System.Collections.ObjectModel;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.DictionaryConverters
{
    internal class SpecializedGenericDictionaryConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(ReadOnlyDictionary<,>), typeof(SortedDictionary<,>),
            typeof(SortedList<,>)];

        public object? Convert(FixtureRequest request, object value, IFixtureContext context)
        {
            var target = request.Type;
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
