using FixtureBuilder.Extensions;
using FixtureBuilder.Helpers;
using System.Collections.Concurrent;

namespace FixtureBuilder.ValueConverters.Converters
{
    internal class BlockingCollectionConverter : IValueConverter
    {
        public object? Convert(Type target, object value)
        {
            if (target.GetGenericTypeDefinitionOrDefault() == typeof(BlockingCollection<>)
                && value.GetType().GetEnumerableElementType() == target.GenericTypeArguments[0])
            {
                if (!value.GetType().Implements(typeof(IProducerConsumerCollection<>)))
                {
                    var intermediateType = typeof(ConcurrentQueue<>).MakeGenericType(target.GetGenericArguments()[0]);
                    value = InstantiationHelpers.UseConstructor(intermediateType, value)!;
                }

                return InstantiationHelpers.UseConstructor(target, value);
            }

            return null;
        }
    }
}
