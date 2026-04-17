using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.FixtureFactories.WithMatching
{
    internal class MatchingProvider : IValueProvider
    {
        private readonly IEnumerable<IWithRule> _withRules;
        private readonly object? _value;

        public MatchingProvider(IEnumerable<IWithRule> withRules, object? value)
        {
            ArgumentNullException.ThrowIfNull(withRules);
            _withRules = withRules;
            _value = value;
        }

        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (_withRules.All(r => r.IsMatch(request)))
                return _value;

            return new NoResult();
        }
    }
}
