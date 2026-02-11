using FixtureBuilder.Extensions;
using System.Collections.Frozen;
using System.Reflection;

namespace FixtureBuilder.ValueConverters.DictionaryConverters
{
    internal class FrozenDictionaryConverter : IValueConverter
    {
        public object? Convert(Type target, object value)
        {
            if (target.GetGenericTypeDefinitionOrDefault() == typeof(FrozenDictionary<,>)
                && value.GetType().GetEnumerableElementType() == target.GetEnumerableElementType())
            {
                var (fieldKeyType, fieldValueType) = GetFieldKeyValueTypes(target);

                var ToFrozenDictionaryMethod = typeof(FrozenDictionary)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m =>
                        m.Name == "ToFrozenDictionary" &&
                        m.IsGenericMethodDefinition &&
                        m.GetParameters().Length == 2)
                    ?? throw new MissingMethodException("Did not find ToFrozenDictionary method for FrozenDictionary.");

                var genericToFrozenDictionary = ToFrozenDictionaryMethod.MakeGenericMethod(fieldKeyType, fieldValueType);

                return genericToFrozenDictionary.Invoke(null, [value, null])!;
            }

            return null;
        }

        //TODO: This should only exist in one place
        private static (Type fieldKeyType, Type fieldValueType) GetFieldKeyValueTypes(Type fieldType)
        {
            Type fieldKeyType = typeof(object);
            Type fieldValueType = typeof(object);

            var fieldGenArgs = fieldType.GetGenericArguments();

            if (fieldGenArgs.Length == 2)
            {
                fieldKeyType = fieldGenArgs[0];
                fieldValueType = fieldGenArgs[1];
            }

            return (fieldKeyType, fieldValueType);
        }
    }
}
