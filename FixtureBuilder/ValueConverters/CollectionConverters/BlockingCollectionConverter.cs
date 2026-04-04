using System.Collections.Concurrent;
using FixtureBuilder.Extensions;
using FixtureBuilder.FixtureContexts;

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
                    value = Activator.CreateInstance(intermediateType, value)!;
                }

                return Activator.CreateInstance(target, value);
            }

            return null;
        }
    }
}
