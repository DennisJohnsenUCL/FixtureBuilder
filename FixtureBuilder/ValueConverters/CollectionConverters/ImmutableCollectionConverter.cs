using FixtureBuilder.Extensions;
using System.Collections;
using System.Collections.Immutable;
using System.Reflection;

namespace FixtureBuilder.ValueConverters.CollectionConverters
{
    internal class ImmutableCollectionConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(ImmutableList<>), typeof(ImmutableHashSet<>),
            typeof(ImmutableStack<>), typeof(ImmutableQueue<>), typeof(ImmutableArray<>), typeof(ImmutableSortedSet<>)];

        public object? Convert(Type target, object value)
        {
            if (_types.Contains(target.GetGenericTypeDefinitionOrDefault())
                && value.GetType().GetEnumerableElementType() == target.GenericTypeArguments[0])
            {
                var elementType = target.GenericTypeArguments[0];
                var genericTypeDef = target.GetGenericTypeDefinition();

                var factoryTypeName = genericTypeDef.FullName!.Replace("`1", "");
                var factoryType = Type.GetType(factoryTypeName + ", System.Collections.Immutable")
                    ?? throw new InvalidOperationException($"Failed to resolve factory type for {target.Name}.");

                var createRangeMethod = factoryType
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m =>
                        m.Name == "CreateRange" &&
                        m.IsGenericMethodDefinition &&
                        m.GetParameters().Length == 1)
                    ?? throw new InvalidOperationException($"Failed to get CreateRange method for {target.Name}.");

                var genericCreateRange = createRangeMethod.MakeGenericMethod(elementType);

                return genericCreateRange.Invoke(null, [value]) as IEnumerable
                    ?? throw new InvalidOperationException($"Failed to create immutable collection for {target.Name}.");
            }

            return null;
        }
    }
}
