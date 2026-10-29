using System.Collections.Concurrent;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.CollectionConverters
{
    internal class MutableGenericCollectionConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(List<>), typeof(Stack<>), typeof(Queue<>),
            typeof(SortedSet<>), typeof(ConcurrentBag<>),
            typeof(ConcurrentQueue<>), typeof(ConcurrentStack<>), typeof(HashSet<>), typeof(LinkedList<>)];

        public object? Convert(TypeRequest request, object value, IFixtureContext context)
        {
            var target = request.Type;
            if (_types.Contains(target.GetGenericTypeDefinitionOrDefault())
                && value.GetType().GetEnumerableElementType() == target.GenericTypeArguments[0])
            {
                return Activator.CreateInstance(target, value);
            }

            return NoResult.Instance;
        }
    }
}
