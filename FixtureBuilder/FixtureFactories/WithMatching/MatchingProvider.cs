using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.FixtureFactories.WithMatching
{
    internal class MatchingProvider : IValueProvider
    {
        private readonly IEnumerable<IWithRule> _withRules;
        private readonly Func<object?> _func;

        public MatchingProvider(IEnumerable<IWithRule> withRules, object? value)
            : this(withRules, () => value) { }

        public MatchingProvider(IEnumerable<IWithRule> withRules, Func<object?> func)
        {
            ArgumentNullException.ThrowIfNull(withRules);
            ArgumentNullException.ThrowIfNull(func);
            _withRules = withRules;
            _func = func;
        }

        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (_withRules.All(r => r.IsMatch(request)))
                return _func();

            return new NoResult();
        }
    }
}
