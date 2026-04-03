using FixtureBuilder.FixtureContexts;
using FixtureBuilder.FixtureProviders;
using FixtureBuilder.FixtureProviders.Providers;

namespace FixtureBuilder.UninitializedProviders
{
    /// <summary>
    /// An <see cref="IFixtureUninitializedProvider"/> that iterates through a sequence of
    /// <see cref="IFixtureProvider"/> instances, returning the first non-null result. If no
    /// provider resolves the request, it falls back to the <see cref="IFixtureContext"/> to
    /// resolve the uninitialized value.
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

            result = context.Resolve(request, context);
            if (result != null) return result;

            result = _defaultBclTypeProvider.Resolve(request, context);
            if (result != null) return result;

            result = context.ResolveUninitialized(request, initializeMembers, context);
            return result;
        }
    }
}
