using FixtureBuilder.Extensions;
using FixtureBuilder.Helpers;
using System.Collections;

namespace FixtureBuilder.ValueConverters.DictionaryConverters
{
    internal class NonGenericDictionaryConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(Hashtable), typeof(SortedList)];

        public object? Convert(Type target, object value)
        {
            //TODO: Add check for isDictionary or ienumerable kvp, same as casting decorator
            if (_types.Contains(target))
            {
                if (value is not IDictionary)
                {
                    var (sourceKeyType, sourceValueType) = GetSourceKeyValueTypes(value);
                    var intermediateType = typeof(Dictionary<,>).MakeGenericType(sourceKeyType, sourceValueType);
                    value = (IEnumerable)InstantiationHelpers.UseConstructor(intermediateType, value)!;
                }

                return InstantiationHelpers.UseConstructor(target, value);
            }
            return null;
        }

        //Can be unified with Field method?
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
                .FirstOrDefault(args => args.Length == 2);

            if (keyValueTypes != null)
            {
                sourceKeyType = keyValueTypes[0];
                sourceValueType = keyValueTypes[1];
            }

            return (sourceKeyType, sourceValueType);
        }
    }
}
