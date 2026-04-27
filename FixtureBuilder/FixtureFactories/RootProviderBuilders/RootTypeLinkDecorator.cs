using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;

namespace FixtureBuilder.FixtureFactories.RootProviderBuilders
{
    internal class RootTypeLinkDecorator : ITypeLink
    {
        private readonly ITypeLink _inner;
        private readonly Type _rootType;

        public RootTypeLinkDecorator(ITypeLink typeLink, Type rootType)
        {
            ArgumentNullException.ThrowIfNull(typeLink);
            ArgumentNullException.ThrowIfNull(rootType);
            _inner = typeLink;
            _rootType = rootType;
        }

        public Type? Link(FixtureRequest request)
        {
            if (request.RootType != _rootType)
                return null;

            return _inner.Link(request);
        }
    }
}
