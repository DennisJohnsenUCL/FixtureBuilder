using FixtureBuilder.Extensions;
using FixtureBuilder.Helpers;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace FixtureBuilder.ValueConverters
{
    internal class MutableGenericCollectionConverter : IValueConverter
    {
        private readonly IEnumerable<Type> _types = [typeof(List<>), typeof(Stack<>), typeof(Queue<>),
            typeof(SortedSet<>), typeof(ReadOnlyCollection<>), typeof(Collection<>), typeof(ConcurrentBag<>),
            typeof(ConcurrentQueue<>), typeof(ConcurrentStack<>), typeof(HashSet<>)];

        public object? Convert(Type target, object value)
        {
            var sourceType = value.GetType();

            if (_types.Contains(target.GetGenericTypeDefinitionOrDefault())
                && sourceType.Implements(typeof(IEnumerable<>))
                && sourceType.GenericTypeArguments[0] == target.GenericTypeArguments[0])
            {
                return InstantiationHelpers.UseConstructor(target, value);
            }

            return null;
        }
    }
}
