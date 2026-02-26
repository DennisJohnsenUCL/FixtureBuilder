using FixtureBuilder.Extensions;
using FixtureBuilder.FixtureContexts;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace FixtureBuilder.FixtureProviders.Providers
{
    internal class ImmutableFrozenEnumerableProvider : IFixtureProvider
    {
        private readonly IEnumerable<Type> _types = [typeof(ImmutableList<>), typeof(ImmutableHashSet<>),
        typeof(ImmutableStack<>), typeof(ImmutableQueue<>), typeof(ImmutableArray<>), typeof(ImmutableSortedSet<>),
        typeof(FrozenSet<>), typeof(ImmutableDictionary<,>), typeof(ImmutableSortedDictionary<,>),
        typeof(FrozenDictionary<,>)];

        public object? Resolve(FixtureRequest request, IFixtureContext context)
        {
            if (_types.Contains(request.Type.GetGenericTypeDefinitionOrDefault()))
            {
                var emptyMethod = request.Type.GetMethod("Empty");
                var instance = emptyMethod?.Invoke(null, []) ?? null;
                if (instance != null) return instance;
            }

            return null;
        }
    }
}
