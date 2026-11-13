using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.CollectionConverters
{
    internal class ArrayConverter : IValueConverter
    {
        private static readonly MethodInfo ToArrayMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray))!;

        public object? Convert(TypeRequest request, object value, IFixtureContext context)
        {
            if (request.Type.IsArray
                && request.Type.GetEnumerableElementType() is { } elementType
                && value.GetType().GetEnumerableElementType() == elementType)
            {
                return ToArrayMethod
                    .MakeGenericMethod(elementType)
                    .Invoke(null, [value]);
            }

            return NoResult.Instance;
        }
    }
}
