using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Assignment.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for immutable and frozen collection types.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Creates empty instances by invoking the static <c>Empty</c> property or method
    /// exposed by each supported type. Supported types include:
    /// </para>
    /// <list type="bullet">
    ///   <item><description><see cref="ImmutableList{T}"/>, <see cref="ImmutableHashSet{T}"/>, <see cref="ImmutableSortedSet{T}"/></description></item>
    ///   <item><description><see cref="ImmutableStack{T}"/>, <see cref="ImmutableQueue{T}"/>, <see cref="ImmutableArray{T}"/></description></item>
    ///   <item><description><see cref="FrozenSet{T}"/></description></item>
    ///   <item><description><see cref="ImmutableDictionary{TKey, TValue}"/>, <see cref="ImmutableSortedDictionary{TKey, TValue}"/></description></item>
    ///   <item><description><see cref="FrozenDictionary{TKey, TValue}"/></description></item>
    /// </list>
    /// <para>
    /// Returns <see langword="null"/> for any type not in the supported set.
    /// </para>
    /// </remarks>
    internal class ImmutableFrozenEnumerableProvider : IValueProvider
    {
        private readonly IEnumerable<Type> _types = [typeof(ImmutableList<>), typeof(ImmutableHashSet<>),
        typeof(ImmutableStack<>), typeof(ImmutableQueue<>), typeof(ImmutableArray<>), typeof(ImmutableSortedSet<>),
        typeof(FrozenSet<>), typeof(ImmutableDictionary<,>), typeof(ImmutableSortedDictionary<,>),
        typeof(FrozenDictionary<,>), typeof(ReadOnlyCollection<>), typeof(ReadOnlyDictionary<,>)];

        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (_types.Contains(request.Type.GetGenericTypeDefinitionOrDefault()))
            {
                var emptyProperty = request.Type.GetProperty("Empty", BindingFlags.Public | BindingFlags.Static);
                var instance = emptyProperty?.GetValue(null);
                if (instance != null) return instance;

                var emptyField = request.Type.GetField("Empty", BindingFlags.Public | BindingFlags.Static);
                instance = emptyField?.GetValue(null);
                if (instance != null) return instance;
            }

            return new NoResult();
        }
    }
}
