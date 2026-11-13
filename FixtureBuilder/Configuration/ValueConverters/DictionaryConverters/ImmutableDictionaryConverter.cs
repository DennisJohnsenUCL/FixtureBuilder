using System.Collections.Immutable;
using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.DictionaryConverters
{
    internal class ImmutableDictionaryConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(ImmutableDictionary<,>), typeof(ImmutableSortedDictionary<,>)];

        public object? Convert(TypeRequest request, object value, IFixtureContext context)
        {
            var target = request.Type;
            if (_types.Contains(target.GetGenericTypeDefinitionOrDefault())
                && value.GetType().GetEnumerableElementType() == target.GetEnumerableElementType())
            {
                var genericTypeDef = target.GetGenericTypeDefinition();

                var (fieldKeyType, fieldValueType) = target.GetDictionaryEnumerableTypes();

                var factoryTypeName = genericTypeDef.FullName!.Replace("`2", "");
                var factoryType = Type.GetType(factoryTypeName + ", System.Collections.Immutable")!;

                var createRangeMethod = factoryType
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .First(m =>
                        m.Name == "CreateRange" &&
                        m.GetParameters().Length == 1);

                var genericCreateRange = createRangeMethod.MakeGenericMethod(fieldKeyType!, fieldValueType!);

                return genericCreateRange.Invoke(null, [value]);
            }
            return NoResult.Instance;
        }
    }
}
