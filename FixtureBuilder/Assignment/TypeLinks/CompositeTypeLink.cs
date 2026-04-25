using FixtureBuilder.Core;

namespace FixtureBuilder.Assignment.TypeLinks
{
    internal class CompositeTypeLink : ICompositeTypeLink
    {
        private IEnumerable<ITypeLink> _typeLinks;

        public CompositeTypeLink(IEnumerable<ITypeLink> typeLinks)
        {
            ArgumentNullException.ThrowIfNull(typeLinks);

            _typeLinks = typeLinks;
        }

        public Type? Link(FixtureRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            foreach (var typeLink in _typeLinks)
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
    }
}
