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

        public object? Convert(FixtureRequest request, object value, IFixtureContext context)
        {
            var target = request.Type;
            if (_types.Contains(target.GetGenericTypeDefinitionOrDefault())
                && value.GetType().GetEnumerableElementType() == target.GetEnumerableElementType())
            {
                var genericTypeDef = target.GetGenericTypeDefinition();

                var (fieldKeyType, fieldValueType) = target.GetDictionaryEnumerableTypes();

                var factoryTypeName = genericTypeDef.FullName!.Replace("`2", "");
                var factoryType = Type.GetType(factoryTypeName + ", System.Collections.Immutable")
                    ?? throw new InvalidOperationException($"Failed to resolve factory type for {target.Name}.");

                var createRangeMethod = factoryType
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m =>
                        m.Name == "CreateRange" &&
                        m.IsGenericMethodDefinition &&
                        m.GetParameters().Length == 1)
                    ?? throw new MissingMethodException("Did not find CreateRange method for ImmutableDictionary.");

                var genericCreateRange = createRangeMethod.MakeGenericMethod(fieldKeyType!, fieldValueType!);

                return genericCreateRange.Invoke(null, [value])!;
            }
            return new NoResult();
        }
    }
}
