using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.FixtureFactories
{
    internal class CustomProviderAdapter : IValueProvider
    {
        private readonly ICustomProvider _adaptee;

        public CustomProviderAdapter(ICustomProvider adaptee)
        {
            ArgumentNullException.ThrowIfNull(adaptee);
            _adaptee = adaptee;
        }

        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            return _adaptee.ResolveValue(request);
        }
    }
}
