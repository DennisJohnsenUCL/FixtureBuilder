namespace FixtureBuilder.Assignment.TypeLinks
{
    internal class CompositeTypeLink : ITypeLink
    {
        private readonly IEnumerable<ITypeLink> _typeLinks;

        public CompositeTypeLink(IEnumerable<ITypeLink> typeLinks)
        {
            ArgumentNullException.ThrowIfNull(typeLinks);

            _typeLinks = typeLinks;
        }

        public Type? Link(Type target)
        {
            ArgumentNullException.ThrowIfNull(target);

            foreach (var typeLink in _typeLinks)
            {
                var result = typeLink.Link(target);
                if (result != null) return result;
            }
            return null;
        }
    }
}
