using System.Collections;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.DictionaryConverters
{
    internal class NonGenericDictionaryConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(Hashtable), typeof(SortedList)];

        public object? Convert(Type target, object value, IFixtureContext context)
        {
            if (_types.Contains(target)
                && (value is IDictionary
                || value.GetType().GetEnumerableElementType()?.GetGenericTypeDefinitionOrDefault() == (typeof(KeyValuePair<,>))))
            {
                if (value is not IDictionary)
                {
                    var (sourceKeyType, sourceValueType) = GetSourceKeyValueTypes(value);
                    var intermediateType = typeof(Dictionary<,>).MakeGenericType(sourceKeyType, sourceValueType);
                    value = (IEnumerable)Activator.CreateInstance(intermediateType, value)!;
                }

                return Activator.CreateInstance(target, value);
            }
            return new NoResult();
        }

        private static (Type sourceKeyType, Type sourceValueType) GetSourceKeyValueTypes(object values)
        {
            Type sourceKeyType = typeof(object);
            Type sourceValueType = typeof(object);

            var keyValueTypes = values.GetType()
                .GetInterfaces()
                .Where(i => i.GetGenericTypeDefinitionOrDefault() == typeof(IEnumerable<>))
                .Select(i => i.GetGenericArguments()[0])
                .Where(t => t.GetGenericTypeDefinitionOrDefault() == typeof(KeyValuePair<,>))
                .Select(t => t.GetGenericArguments())
                .FirstOrDefault();

            if (keyValueTypes != null)
            {
                sourceKeyType = keyValueTypes[0];
                sourceValueType = keyValueTypes[1];
            }

            return (sourceKeyType, sourceValueType);
        }
    }
}
