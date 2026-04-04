using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using FixtureBuilder.Extensions;
using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.ValueConverters.CollectionConverters
{
    internal class MutableGenericCollectionConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(List<>), typeof(Stack<>), typeof(Queue<>),
            typeof(SortedSet<>), typeof(ReadOnlyCollection<>), typeof(Collection<>), typeof(ConcurrentBag<>),
            typeof(ConcurrentQueue<>), typeof(ConcurrentStack<>), typeof(HashSet<>), typeof(LinkedList<>)];

        public object? Convert(Type target, object value, IFixtureContext context)
        {
            if (_types.Contains(target.GetGenericTypeDefinitionOrDefault())
                && value.GetType().GetEnumerableElementType() == target.GenericTypeArguments[0])
            {
                return Activator.CreateInstance(target, value);
            }

            return null;
        }
    }
}
