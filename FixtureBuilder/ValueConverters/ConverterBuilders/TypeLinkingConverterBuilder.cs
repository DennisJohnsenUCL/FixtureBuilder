using FixtureBuilder.TypeLinks;
using FixtureBuilder.ValueConverters.Decorators;
using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace FixtureBuilder.ValueConverters.ConverterBuilders
{
    internal class TypeLinkingConverterBuilder
    {
        private readonly ConverterBuilder _builder;
        private IEnumerable<ITypeLink> _typeLinks = [];

        public TypeLinkingConverterBuilder(ConverterBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            _builder = builder;
        }

        public TypeLinkingConverterBuilder AddDictionaryTypeLinks()
        {
            _typeLinks = _typeLinks.Concat([new TypeLink(typeof(IDictionary<,>), typeof(Dictionary<,>)),
                new TypeLink(typeof(IImmutableDictionary<,>), typeof(ImmutableDictionary<,>)),
                new TypeLink(typeof(IReadOnlyDictionary<,>), typeof(ReadOnlyDictionary<,>)),
                new TypeLink(typeof(IDictionary), typeof(Hashtable))]);

            return this;
        }

        public TypeLinkingConverterBuilder AddEnumerableTypeLinks()
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

        public ConverterBuilder And()
        {
            var inner = _builder.Build();
            var typeLinks = new CompositeTypeLink(_typeLinks);
            _builder.WithDecorator(new TypeLinkingConverter(inner, typeLinks));
            return _builder;
        }
    }
}
