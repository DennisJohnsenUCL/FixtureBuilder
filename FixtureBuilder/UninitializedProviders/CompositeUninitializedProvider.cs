using FixtureBuilder.FixtureContexts;
using FixtureBuilder.FixtureProviders;

namespace FixtureBuilder.UninitializedProviders
{
    internal class CompositeUninitializedProvider : IFixtureUninitializedProvider
    {
        private readonly IEnumerable<IFixtureProvider> _providers;

        public CompositeUninitializedProvider(IEnumerable<IFixtureProvider> providers)
        {
            ArgumentNullException.ThrowIfNull(providers);
            _providers = providers;
        }

        public object? ResolveUninitialized(FixtureRequest target, InitializeMembers initializeMembers, IFixtureContext context)
        {
            object? result;
            foreach (var provider in _providers)
            {
                result = provider.Resolve(target, context);
                if (result != null) return result;
            }

            result = context.ResolveUninitialized(target, initializeMembers, context);
            return result;
        }
    }
}
