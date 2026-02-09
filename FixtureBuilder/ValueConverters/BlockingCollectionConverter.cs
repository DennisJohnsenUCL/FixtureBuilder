using FixtureBuilder.Extensions;
using FixtureBuilder.Helpers;
using System.Collections.Concurrent;

namespace FixtureBuilder.ValueConverters
{
    internal class BlockingCollectionConverter : IValueConverter
    {
        public object? Convert(Type target, object value)
        {
            ArgumentNullException.ThrowIfNull(target);
            if (value == null) return null;

            var sourceType = value.GetType();
            if (sourceType == target) return value;

            if (target.GetGenericTypeDefinitionOrDefault() == typeof(BlockingCollection<>)
                && sourceType.GetEnumerableElementType() == target.GenericTypeArguments[0])
            {
                var intermediateType = typeof(ConcurrentBag<>).MakeGenericType(target.GetGenericArguments()[0]);
                var intermediate = InstantiationHelpers.UseConstructor(intermediateType, value)!;
                return InstantiationHelpers.UseConstructor(target, intermediate);
            }

            return null;
        }
    }
}
