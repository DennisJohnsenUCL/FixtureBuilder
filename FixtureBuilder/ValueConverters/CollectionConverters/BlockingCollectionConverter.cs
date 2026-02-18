using FixtureBuilder.Extensions;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.Helpers;
using System.Collections.Concurrent;

namespace FixtureBuilder.ValueConverters.CollectionConverters
{
    internal class BlockingCollectionConverter : IValueConverter
    {
        public object? Convert(Type target, object value, IFixtureContext context)
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
