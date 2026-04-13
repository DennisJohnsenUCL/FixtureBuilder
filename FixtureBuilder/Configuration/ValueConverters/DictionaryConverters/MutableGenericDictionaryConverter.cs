using System.Collections.Concurrent;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.DictionaryConverters
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
