using System.Collections.Frozen;
using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.DictionaryConverters
{
    internal class FrozenDictionaryConverter : IValueConverter
    {
        public object? Convert(Type target, object value, IFixtureContext context)
        {
            if (target.GetGenericTypeDefinitionOrDefault() == typeof(FrozenDictionary<,>)
                && value.GetType().GetEnumerableElementType() == target.GetEnumerableElementType())
            {
                var (fieldKeyType, fieldValueType) = target.GetDictionaryEnumerableTypes();

                var ToFrozenDictionaryMethod = typeof(FrozenDictionary)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m =>
                        m.Name == "ToFrozenDictionary" &&
                        m.IsGenericMethodDefinition &&
                        m.GetParameters().Length == 2)
                    ?? throw new MissingMethodException("Did not find ToFrozenDictionary method for FrozenDictionary.");

                var genericToFrozenDictionary = ToFrozenDictionaryMethod.MakeGenericMethod(fieldKeyType!, fieldValueType!);

                return genericToFrozenDictionary.Invoke(null, [value, null])!;
            }

            return new NoResult();
        }
    }
}
