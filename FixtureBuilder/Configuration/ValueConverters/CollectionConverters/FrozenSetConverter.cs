using System.Collections.Frozen;
using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.CollectionConverters
{
    internal class FrozenSetConverter : IValueConverter
    {
        private static readonly MethodInfo ToFrozenSetMethod = typeof(FrozenSet).GetMethod(nameof(FrozenSet.ToFrozenSet))!;

        public object? Convert(TypeRequest request, object value, IFixtureContext context)
        {
            var target = request.Type;
            if (target.GetGenericTypeDefinitionOrDefault() == typeof(FrozenSet<>)
                && value.GetType().GetEnumerableElementType() == target.GenericTypeArguments[0])
            {
                var elementType = target.GenericTypeArguments[0];

                return ToFrozenSetMethod
                    .MakeGenericMethod(elementType)
                    .Invoke(null, [value, null]);
            }

            return NoResult.Instance;
        }
    }
}
