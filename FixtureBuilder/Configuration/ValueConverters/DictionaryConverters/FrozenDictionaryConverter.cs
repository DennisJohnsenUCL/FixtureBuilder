using System.Collections.Frozen;
using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.DictionaryConverters
{
    internal class FrozenDictionaryConverter : IValueConverter
    {
        private static readonly MethodInfo ToFrozenDictionaryMethod = typeof(FrozenDictionary)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .First(m => m.Name == nameof(FrozenDictionary.ToFrozenDictionary)
                    && m.GetParameters().Length == 2);


        public object? Convert(TypeRequest request, object value, IFixtureContext context)
        {
            var target = request.Type;
            if (target.GetGenericTypeDefinitionOrDefault() == typeof(FrozenDictionary<,>)
                && value.GetType().GetEnumerableElementType() == target.GetEnumerableElementType())
            {
                var (fieldKeyType, fieldValueType) = target.GetDictionaryEnumerableTypes();

                return ToFrozenDictionaryMethod
                    .MakeGenericMethod(fieldKeyType!, fieldValueType!)
                    .Invoke(null, [value, null]);
            }

            return NoResult.Instance;
        }
    }
}
