using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.FixtureProviders
{
    /// <summary>
    /// An <see cref="IFixtureProvider"/> that iterates through a sequence of
    /// <see cref="IFixtureProvider"/> instances, returning the first non-null result.
    /// Returns <see langword="null"/> if no provider resolves the request.
    /// </summary>
    internal class CompositeFixtureProvider : IFixtureProvider
    {
        private readonly IEnumerable<IFixtureProvider> _providers;

        public CompositeFixtureProvider(IEnumerable<IFixtureProvider> providers)
        {
            ArgumentNullException.ThrowIfNull(providers);

            _providers = providers;
        }

        public object? Resolve(FixtureRequest request, IFixtureContext context)
        {
            foreach (var provider in _providers)
            {
                var result = provider.Resolve(request, context);
                if (result != null) return result;
            }

            return null;
        }
    }
}
