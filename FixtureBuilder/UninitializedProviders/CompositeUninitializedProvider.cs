using FixtureBuilder.FixtureContexts;
using FixtureBuilder.ValueProviders.Providers;

namespace FixtureBuilder.UninitializedProviders
{
    /// <summary>
    /// The default <see cref="IFixtureUninitializedProvider"/> implementation that defines the
    /// resolution pipeline for uninitialized fixture values. Attempts resolution in order:
    /// first via the <see cref="IFixtureContext"/>'s registered providers, then via
    /// <see cref="DefaultBclTypeProvider"/> for <c>System</c>-namespace types, and finally
    /// via <see cref="IFixtureContext.ResolveUninitialized"/> for direct construction.
    /// </summary>
    internal class CompositeUninitializedProvider : IFixtureUninitializedProvider
    {
        private readonly DefaultBclTypeProvider _defaultBclTypeProvider;

        public CompositeUninitializedProvider(DefaultBclTypeProvider defaultBclTypeProvider)
        {
            ArgumentNullException.ThrowIfNull(defaultBclTypeProvider);

            _defaultBclTypeProvider = defaultBclTypeProvider;
        }

        public object? ResolveUninitialized(FixtureRequest request, InitializeMembers initializeMembers, IFixtureContext context)
        {
            object? result;

            result = context.ResolveValue(request, context);
            if (result != null) return result;

            result = _defaultBclTypeProvider.ResolveValue(request, context);
            if (result != null) return result;

            result = context.ResolveUninitialized(request, initializeMembers, context);

            if (!context.Options.AllowSkipUninitializableMembers && result == null)
                throw new InvalidOperationException($"Could not get a value for or instantiate {request.Type.Name} uninitialized.");

            return result;
        }
    }
}
