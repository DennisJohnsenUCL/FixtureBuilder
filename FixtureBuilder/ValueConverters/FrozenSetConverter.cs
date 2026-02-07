using FixtureBuilder.Extensions;
using System.Collections;
using System.Collections.Frozen;
using System.Reflection;

namespace FixtureBuilder.ValueConverters
{
    internal class FrozenSetConverter : IValueConverter
    {
        public object? Convert(Type target, object value)
        {
            var sourceType = value.GetType();

            if (target.GetGenericTypeDefinitionOrDefault() == typeof(FrozenSet<>)
                && sourceType.Implements(typeof(IEnumerable<>))
                && sourceType.IsGenericType
                && sourceType.GenericTypeArguments[0] == target.GenericTypeArguments[0])
            {
                var elementType = target.GenericTypeArguments[0];

                var ToFrozenSetMethod = typeof(FrozenSet)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name == "ToFrozenSet" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2)
                ?? throw new InvalidOperationException($"Failed to get ToFrozenSet method for {target.Name}.");

                var genericToFrozenSet = ToFrozenSetMethod.MakeGenericMethod(elementType);

                return genericToFrozenSet.Invoke(null, [value, null]) as IEnumerable
                    ?? throw new InvalidOperationException($"Failed to create FrozenSet collection for {target.Name}.");
            }

            return null;
        }
    }
}
