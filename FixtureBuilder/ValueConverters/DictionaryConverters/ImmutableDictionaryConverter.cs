using FixtureBuilder.Extensions;
using System.Collections.Immutable;
using System.Reflection;

namespace FixtureBuilder.ValueConverters.DictionaryConverters
{
    internal class ImmutableDictionaryConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(ImmutableDictionary<,>), typeof(ImmutableSortedDictionary<,>)];

        public object? Convert(Type target, object value)
        {
            if (_types.Contains(target.GetGenericTypeDefinitionOrDefault())
                && value.GetType().GetEnumerableElementType() == target.GetEnumerableElementType())
            {
                var genericTypeDef = target.GetGenericTypeDefinition();

                var (fieldKeyType, fieldValueType) = GetFieldKeyValueTypes(target);

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

                var genericCreateRange = createRangeMethod.MakeGenericMethod(fieldKeyType, fieldValueType);

                return genericCreateRange.Invoke(null, [value])!;
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
