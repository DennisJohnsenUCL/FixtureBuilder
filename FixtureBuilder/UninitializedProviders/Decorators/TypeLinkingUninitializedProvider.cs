using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.UninitializedProviders.Decorators
{
    internal class TypeLinkingUninitializedProvider : IFixtureUninitializedProvider
    {
        private readonly IFixtureUninitializedProvider _inner;

        public TypeLinkingUninitializedProvider(IFixtureUninitializedProvider inner)
        {
            ArgumentNullException.ThrowIfNull(inner);
            _inner = inner;
        }

        public object? ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context)
        {
            var nullableType = Nullable.GetUnderlyingType(request.Type);
            if (nullableType != null) request.Type = nullableType;

            var link = context.Link(request.Type);
            if (link != null) request.Type = link;

            return _inner.ResolveUninitialized(request, initializeMembers, context);
        }
    }
}
