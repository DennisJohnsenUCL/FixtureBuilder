using System.Collections.Immutable;
using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.CollectionConverters
{
    internal class ImmutableCollectionConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(ImmutableList<>), typeof(ImmutableHashSet<>),
            typeof(ImmutableStack<>), typeof(ImmutableQueue<>), typeof(ImmutableArray<>), typeof(ImmutableSortedSet<>)];

        public object? Convert(TypeRequest request, object value, IFixtureContext context)
        {
            var target = request.Type;
            if (_types.Contains(target.GetGenericTypeDefinitionOrDefault())
                && value.GetType().GetEnumerableElementType() == target.GenericTypeArguments[0])
            {
                var elementType = target.GenericTypeArguments[0];
                var genericTypeDef = target.GetGenericTypeDefinition();

                var factoryTypeName = genericTypeDef.FullName!.Replace("`1", "");
                var factoryType = Type.GetType(factoryTypeName + ", System.Collections.Immutable")!;

                var createRangeMethod = factoryType
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .First(m =>
                        m.Name == "CreateRange" &&
                        m.GetParameters().Length == 1);

                var genericCreateRange = createRangeMethod.MakeGenericMethod(elementType);

                return genericCreateRange.Invoke(null, [value]);
            }

            return NoResult.Instance;
        }
    }
}
