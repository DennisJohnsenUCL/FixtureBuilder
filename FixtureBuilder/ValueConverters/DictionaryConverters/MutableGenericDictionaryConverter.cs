using System.Collections.Concurrent;
using FixtureBuilder.Extensions;
using FixtureBuilder.FixtureContexts;

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
                return Activator.CreateInstance(target, value);
            }
            return new NoResult();
        }
    }
}
