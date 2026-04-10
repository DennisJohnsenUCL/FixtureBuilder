using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.UninitializedProviders.Decorators
{
    /// <summary>
    /// A decorator over <see cref="IUninitializedProvider"/> that resolves type links
    /// before delegating to the inner provider. If the requested type is a nullable value type, it first
    /// unwraps it to its underlying type. It then consults the <see cref="IFixtureContext"/>
    /// for a linked type, replacing the request type if a link is found.
    /// </summary>
    internal class TypeLinkingUninitializedProvider : IUninitializedProvider
    {
        private readonly IUninitializedProvider _inner;

        public TypeLinkingUninitializedProvider(IUninitializedProvider inner)
        {
            ArgumentNullException.ThrowIfNull(inner);
            _inner = inner;
        }

        public object? ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context, RecursiveResolveContext? recursiveResolveContext)
        {
            var link = context.Link(request.Type);
            if (link != null) request.Type = link;

            return _inner.ResolveUninitialized(request, initializeMembers, context, recursiveResolveContext);
        }
    }
}
