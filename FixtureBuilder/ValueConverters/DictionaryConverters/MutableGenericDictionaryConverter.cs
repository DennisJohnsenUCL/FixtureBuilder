using FixtureBuilder.Extensions;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.Helpers;
using System.Collections.Concurrent;

namespace FixtureBuilder.ValueConverters.DictionaryConverters
{
    internal class MutableGenericDictionaryConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(Dictionary<,>), typeof(ConcurrentDictionary<,>),
            typeof(OrderedDictionary<,>)];

        public object? Convert(Type target, object value, IFixtureContext context)
        {
            if (_types.Contains(target.GetGenericTypeDefinitionOrDefault())
                && value.GetType().GetEnumerableElementType() == target.GetEnumerableElementType())
            {
                return InstantiationHelpers.UseConstructor(target, value);
            }
            return null;
        }
    }
}
