using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Creation.UninitializedProviders
{
    /// <summary>
    /// The default <see cref="IUninitializedProvider"/> implementation that defines the
    /// resolution pipeline for uninitialized fixture values. Attempts resolution in order:
    /// first via the <see cref="IFixtureContext"/>'s registered providers, then via
    /// <see cref="DefaultBclTypeProvider"/> for <c>System</c>-namespace types, and finally
    /// via <see cref="IFixtureContext.ResolveUninitialized"/> for direct construction.
    /// </summary>
    internal class CompositeUninitializedProvider : IUninitializedProvider
    {
        private readonly DefaultBclTypeProvider _defaultBclTypeProvider;

        public CompositeUninitializedProvider(DefaultBclTypeProvider defaultBclTypeProvider)
        {
            ArgumentNullException.ThrowIfNull(defaultBclTypeProvider);

            _defaultBclTypeProvider = defaultBclTypeProvider;
        }

        public object? ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context, RecursiveResolveContext? recursiveResolveContext)
        {
            request.Type = context.UnwrapAndLink(request.Type);

            var result = context.ValueProvider.ResolveValue(request, context);
            if (result is not NoResult) return result;

            result = _defaultBclTypeProvider.ResolveValue(request, context);
            if (result is not NoResult) return result;

            result = context.UninitializedProvider.ResolveUninitialized(request, initializeMembers, context, recursiveResolveContext);

            if (!context.Options.AllowSkipUninitializableMembers && result is NoResult)
                throw new InvalidOperationException($"Could not get a value for or instantiate {request.Type.Name} uninitialized.");

            return result;
        }
    }
}
