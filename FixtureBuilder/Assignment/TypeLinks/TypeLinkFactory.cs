using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace FixtureBuilder.Assignment.TypeLinks
{
    internal class TypeLinkFactory
    {
        public static ICompositeTypeLink CreateDefaultTypeLink()
        {
            var typeLink = new CompositeTypeLink([
                //Non-dictionary enumerables
                new TypeLink(typeof(IEnumerable<>), typeof(List<>)),
                new TypeLink(typeof(IList<>), typeof(List<>)),
                new TypeLink(typeof(IReadOnlyList<>), typeof(List<>)),
                new TypeLink(typeof(ICollection<>), typeof(List<>)),
                new TypeLink(typeof(IReadOnlyCollection<>), typeof(List<>)),
                new TypeLink(typeof(ISet<>), typeof(HashSet<>)),
                new TypeLink(typeof(IReadOnlySet<>), typeof(HashSet<>)),
                new TypeLink(typeof(IImmutableList<>), typeof(ImmutableList<>)),
                new TypeLink(typeof(IImmutableStack<>), typeof(ImmutableStack<>)),
                new TypeLink(typeof(IImmutableQueue<>), typeof(ImmutableQueue<>)),
                new TypeLink(typeof(IImmutableSet<>), typeof(ImmutableHashSet<>)),
                new TypeLink(typeof(IList), typeof(ArrayList)),
                new TypeLink(typeof(ICollection), typeof(ArrayList)),
                new TypeLink(typeof(IEnumerable), typeof(ArrayList)),
                
                //Dictionaries
                new TypeLink(typeof(IDictionary<,>), typeof(Dictionary<,>)),
                new TypeLink(typeof(IImmutableDictionary<,>), typeof(ImmutableDictionary<,>)),
                new TypeLink(typeof(IReadOnlyDictionary<,>), typeof(ReadOnlyDictionary<,>)),
                new TypeLink(typeof(IDictionary), typeof(Hashtable))]);

            return typeLink;
        }
    }
}
