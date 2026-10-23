using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Assignment.ValueProviders
{
    /// <summary>
    /// An <see cref="IValueProvider"/> that iterates through a sequence of
    /// <see cref="IValueProvider"/> instances, returning the first result that is not <see cref="NoResult"/>.
    /// Root-scoped providers are always tried before global and built-in ones, regardless of registration order.
    /// Returns <see cref="NoResult"/> if no provider resolves the request.
    /// </summary>
    internal class CompositeValueProvider : ICompositeValueProvider
    {
        private IEnumerable<IValueProvider> _providers;
        private IEnumerable<IValueProvider> _rootProviders;

        public CompositeValueProvider(IEnumerable<IValueProvider> providers)
        {
            ArgumentNullException.ThrowIfNull(providers);

            _providers = providers;
            _rootProviders = [];
        }

        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            var originalType = request.OriginalType.IsByRef ? request.OriginalType.GetElementType()! : request.OriginalType;
            var isValueTypeNotNullable = originalType.IsValueType
                && originalType.GetGenericTypeDefinitionOrDefault() != typeof(Nullable<>);

            foreach (var provider in _rootProviders.Concat(_providers))
            {
                var result = provider.ResolveValue(request, context);

                if (result == null && isValueTypeNotNullable)
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

        public void AddRootProvider(IValueProvider rootProvider)
        {
            _rootProviders = _rootProviders.Prepend(rootProvider);
        }
    }
}
