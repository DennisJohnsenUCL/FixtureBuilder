using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.UninitializedProviders.MemberInitialization;

namespace FixtureBuilder.Creation.UninitializedProviders.MemberResolution
{
    /// <summary>
    /// The <see cref="ICompositeUninitializedProvider"/> used by <see cref="MemberInitializer"/> to resolve
    /// member values. Attempts resolution in order:
    /// first via the <see cref="IFixtureContext"/>'s registered providers, then via
    /// <see cref="DefaultBclTypeProvider"/> for <c>System</c>-namespace types, and finally
    /// via <see cref="IFixtureContext.UninitializedProvider"/> for direct construction.
    /// Throws an <see cref="InvalidOperationException"/> if none of them produces a value and
    /// <see cref="FixtureOptions.AllowSkipUninitializableMembers"/> is disabled.
    /// </summary>
    internal class CompositeUninitializedProvider : ICompositeUninitializedProvider
    {
        private readonly DefaultBclTypeProvider _defaultBclTypeProvider;

        public CompositeUninitializedProvider(DefaultBclTypeProvider defaultBclTypeProvider)
        {
            ArgumentNullException.ThrowIfNull(defaultBclTypeProvider);

            _defaultBclTypeProvider = defaultBclTypeProvider;
        }

        public object? ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context, RecursiveResolveContext recursiveResolveContext)
        {
            var result = context.ValueProvider.ResolveValue(request, context);
            if (result is not NoResult) return result;

            result = _defaultBclTypeProvider.ResolveValue(request, context);
            if (result is not NoResult) return result;

            var typeRequest = new TypeRequest(request.Type, request.RootType);
            result = context.UninitializedProvider.ResolveUninitialized(typeRequest, initializeMembers, context, recursiveResolveContext);

            if (!context.OptionsFor(request.RootType).AllowSkipUninitializableMembers && result is NoResult)
                throw new InvalidOperationException($"Could not get a value for or instantiate {request.Type.Name} uninitialized.");

            return result;
        }
    }
}
