using FixtureBuilder.Extensions;
using FixtureBuilder.FixtureContexts;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace FixtureBuilder.FixtureProviders.Providers
{
    internal class ConstructibleEnumerableProvider : IFixtureProvider
    {
        private readonly IEnumerable<Type> _types = [typeof(List<>), typeof(Stack<>), typeof(Queue<>),
            typeof(SortedSet<>), typeof(ReadOnlyCollection<>), typeof(Collection<>), typeof(ConcurrentBag<>),
            typeof(ConcurrentQueue<>), typeof(ConcurrentStack<>), typeof(HashSet<>), typeof(LinkedList<>),
            typeof(ArrayList), typeof(Stack), typeof(Queue), typeof(Dictionary<,>), typeof(ConcurrentDictionary<,>),
            typeof(OrderedDictionary<,>), typeof(Hashtable), typeof(SortedList), typeof(ReadOnlyDictionary<,>),
            typeof(SortedDictionary<,>), typeof(SortedList<,>)];

        public object? Resolve(FixtureRequest request, IFixtureContext context)
        {
            var genericTypeDef = request.Type.GetGenericTypeDefinitionOrDefault();
            var typeToCompare = genericTypeDef ?? request.Type;

            if (_types.Contains(typeToCompare))
            {
                var instance = Activator.CreateInstance(request.Type);
                if (instance != null) return instance;
            }

            return null;
        }
    }
}
