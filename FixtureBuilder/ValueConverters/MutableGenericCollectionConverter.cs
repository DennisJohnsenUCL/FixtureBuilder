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
            ArgumentNullException.ThrowIfNull(target);
            if (value == null) return null;

            var sourceType = value.GetType();
            if (sourceType == target) return value;

            if (_types.Contains(target.GetGenericTypeDefinitionOrDefault())
                && sourceType.GetEnumerableElementType() == target.GenericTypeArguments[0])
            {
                return InstantiationHelpers.UseConstructor(target, value);
            }

            return null;
        }
    }
}
