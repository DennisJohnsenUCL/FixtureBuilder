using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Assignment.ValueProviders
{
    /// <summary>
    /// An <see cref="IValueProvider"/> that iterates through a sequence of
    /// <see cref="IValueProvider"/> instances, returning the first non-null result.
    /// Returns <see langword="null"/> if no provider resolves the request.
    /// </summary>
    internal class CompositeValueProvider : ICompositeValueProvider
    {
        private IEnumerable<IValueProvider> _providers;

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

                if (result == null
                    && request.Type.IsValueType
                    && request.Type.GetGenericTypeDefinitionOrDefault() != typeof(Nullable<>))
                {
                    throw new InvalidOperationException($"ValueProvider {provider.GetType().Name} returned null for a non-nullable value type. " +
                        $"This is not allowed as null cannot be assigned to non-nullable value types.");
                }

                if (result is not NoResult) return result;
            }

            return new NoResult();
        }

        public void AddProvider(IValueProvider provider)
        {
            _providers = _providers.Prepend(provider);
        }
    }
}
