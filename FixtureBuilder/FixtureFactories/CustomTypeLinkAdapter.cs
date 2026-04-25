using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;

namespace FixtureBuilder.FixtureFactories
{
    internal class CustomTypeLinkAdapter : ITypeLink
    {
        private readonly ICustomTypeLink _typeLink;

        public CustomTypeLinkAdapter(ICustomTypeLink typeLink)
        {
            ArgumentNullException.ThrowIfNull(typeLink);
            _typeLink = typeLink;
        }

        public Type? Link(FixtureRequest request)
        {
            return _typeLink.Link(request.Type);
        }
    }
}
