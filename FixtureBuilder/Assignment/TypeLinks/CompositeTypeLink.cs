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
        private readonly List<ITypeLink> _typeLinks;
        private readonly List<ITypeLink> _rootTypeLinks;

        public CompositeTypeLink(IEnumerable<ITypeLink> typeLinks)
        {
            ArgumentNullException.ThrowIfNull(typeLinks);

            _typeLinks = [.. typeLinks];
            _rootTypeLinks = [];
        }

        public Type? Link(TypeRequest request)
        {
            foreach (var typeLink in _rootTypeLinks.Concat(_typeLinks))
            {
                var result = typeLink.Link(request);
                if (result != null) return result;
            }
            return null;
        }

        public void AddTypeLink(ITypeLink link)
        {
            _typeLinks.Insert(0, link);
        }

        public void AddRootTypeLink(ITypeLink rootLink)
        {
            _rootTypeLinks.Insert(0, rootLink);
        }
    }
}
