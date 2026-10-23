using FixtureBuilder.Core;

namespace FixtureBuilder.Assignment.TypeLinks
{
    /// <summary>
    /// An <see cref="ITypeLink"/> that iterates through a sequence of
    /// <see cref="ITypeLink"/> instances, returning the first non-null linked type.
    /// Root-scoped type links are always tried before global and built-in ones, regardless of registration order.
    /// Returns <see langword="null"/> if no type link matches the request.
    /// </summary>
    internal class CompositeTypeLink : ICompositeTypeLink
    {
        private IEnumerable<ITypeLink> _typeLinks;
        private IEnumerable<ITypeLink> _rootTypeLinks;

        public CompositeTypeLink(IEnumerable<ITypeLink> typeLinks)
        {
            ArgumentNullException.ThrowIfNull(typeLinks);

            _typeLinks = typeLinks;
            _rootTypeLinks = [];
        }

        public Type? Link(FixtureRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            foreach (var typeLink in _rootTypeLinks.Concat(_typeLinks))
            {
                var result = typeLink.Link(request);
                if (result != null) return result;
            }
            return null;
        }

        public void AddTypeLink(ITypeLink link)
        {
            _typeLinks = _typeLinks.Prepend(link);
        }

        public void AddRootTypeLink(ITypeLink rootLink)
        {
            _rootTypeLinks = _rootTypeLinks.Prepend(rootLink);
        }
    }
}
