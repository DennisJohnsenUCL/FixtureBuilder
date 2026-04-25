using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;

namespace FixtureBuilder.FixtureFactories.WithMatching
{
    internal class RootTypeLinkDecorator : ITypeLink
    {
        private readonly ITypeLink _typeLink;
        private readonly Type _rootType;

        public RootTypeLinkDecorator(ITypeLink typeLink, Type rootType)
        {
            ArgumentNullException.ThrowIfNull(typeLink);
            ArgumentNullException.ThrowIfNull(rootType);
            _typeLink = typeLink;
            _rootType = rootType;
        }

        public Type? Link(FixtureRequest request)
        {
            if (request.RootType != _rootType)
                return null;

            return _typeLink.Link(request);
        }
    }
}
