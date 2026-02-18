using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace FixtureBuilder.TypeLinks.TypeLinkBuilders
{
    internal class CompositeTypeLinkBuilder
    {
        private readonly TypeLinkBuilder _builder;
        private IEnumerable<ITypeLink> _typeLinks = [];

        public CompositeTypeLinkBuilder(TypeLinkBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            _builder = builder;
        }

        public CompositeTypeLinkBuilder AddEnumerableTypeLinks()
        {
            _typeLinks = _typeLinks.Concat([new TypeLink(typeof(IEnumerable<>), typeof(List<>)),
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
                new TypeLink(typeof(IEnumerable), typeof(ArrayList))]);

            return this;
        }

        public CompositeTypeLinkBuilder AddDictionaryTypeLinks()
        {
            _typeLinks = _typeLinks.Concat([new TypeLink(typeof(IDictionary<,>), typeof(Dictionary<,>)),
                new TypeLink(typeof(IImmutableDictionary<,>), typeof(ImmutableDictionary<,>)),
                new TypeLink(typeof(IReadOnlyDictionary<,>), typeof(ReadOnlyDictionary<,>)),
                new TypeLink(typeof(IDictionary), typeof(Hashtable))]);

            return this;
        }

        public TypeLinkBuilder And()
        {
            _builder.WithDecorator(new CompositeTypeLink(_typeLinks));
            return _builder;
        }
    }
}
