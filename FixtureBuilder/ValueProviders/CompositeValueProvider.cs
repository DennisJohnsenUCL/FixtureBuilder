using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.ValueProviders
{
    /// <summary>
    /// An <see cref="IValueProvider"/> that iterates through a sequence of
    /// <see cref="IValueProvider"/> instances, returning the first non-null result.
    /// Returns <see langword="null"/> if no provider resolves the request.
    /// </summary>
    internal class CompositeValueProvider : IValueProvider
    {
        private readonly IEnumerable<IValueProvider> _providers;

        public CompositeValueProvider(IEnumerable<IValueProvider> providers)
        {
            ArgumentNullException.ThrowIfNull(providers);

            _providers = providers;
        }

        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            foreach (var provider in _providers)
            {
                var result = provider.ResolveValue(request, context);
                if (result != null) return result;
            }

            return null;
        }
    }
}
