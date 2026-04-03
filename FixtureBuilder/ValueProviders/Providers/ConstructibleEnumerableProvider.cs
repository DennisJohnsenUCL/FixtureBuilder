using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using FixtureBuilder.Extensions;
using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for common constructible enumerable and collection types.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Handles both generic and non-generic collection types that can be instantiated via
    /// a parameterless constructor. Supported types include:
    /// </para>
    /// <list type="bullet">
    ///   <item><description><see cref="List{T}"/>, <see cref="HashSet{T}"/>, <see cref="SortedSet{T}"/>, <see cref="LinkedList{T}"/></description></item>
    ///   <item><description><see cref="Stack{T}"/>, <see cref="Queue{T}"/></description></item>
    ///   <item><description><see cref="Collection{T}"/>, <see cref="ReadOnlyCollection{T}"/></description></item>
    ///   <item><description><see cref="ConcurrentBag{T}"/>, <see cref="ConcurrentQueue{T}"/>, <see cref="ConcurrentStack{T}"/></description></item>
    ///   <item><description><see cref="Dictionary{TKey, TValue}"/>, <see cref="ConcurrentDictionary{TKey, TValue}"/>, <see cref="OrderedDictionary{TKey, TValue}"/></description></item>
    ///   <item><description><see cref="ReadOnlyDictionary{TKey, TValue}"/>, <see cref="SortedDictionary{TKey, TValue}"/>, <see cref="SortedList{TKey, TValue}"/></description></item>
    ///   <item><description><see cref="ArrayList"/>, <see cref="Stack"/>, <see cref="Queue"/>, <see cref="Hashtable"/>, <see cref="SortedList"/></description></item>
    /// </list>
    /// <para>
    /// Returns <see langword="null"/> for any type not in the supported set.
    /// </para>
    /// </remarks>
    internal class ConstructibleEnumerableProvider : IValueProvider
    {
        private readonly IEnumerable<Type> _types = [typeof(List<>), typeof(Stack<>), typeof(Queue<>),
            typeof(SortedSet<>), typeof(Collection<>), typeof(ConcurrentBag<>),
            typeof(ConcurrentQueue<>), typeof(ConcurrentStack<>), typeof(HashSet<>), typeof(LinkedList<>),
            typeof(ArrayList), typeof(Stack), typeof(Queue), typeof(Dictionary<,>), typeof(ConcurrentDictionary<,>),
            typeof(OrderedDictionary<,>), typeof(Hashtable), typeof(SortedList),
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
