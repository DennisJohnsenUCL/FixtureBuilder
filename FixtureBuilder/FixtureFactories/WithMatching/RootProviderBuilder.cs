using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.FixtureFactories.WithMatching
{
    internal class RootProviderBuilder<TRoot> : IRootProviderBuilder<TRoot>
    {
        private readonly IFixtureContext _context;

        private readonly List<MatchingProvider> _providers = [];
        internal IReadOnlyList<MatchingProvider> Providers => _providers.AsReadOnly();

        private readonly MatchingProviderBuilder _innerBuilder = new([new RootTypeRule(typeof(TRoot))]);

        public FixtureOptions Options { set { _context.AddRootOptions(typeof(TRoot), value); } }

        public RootProviderBuilder(IFixtureContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            _context = context;
        }

        public void SetOptions(Action<FixtureOptions> optionsAction)
        {
            var options = _context.GetBaseOptions().Clone();
            optionsAction(options);
            _context.AddRootOptions(typeof(TRoot), options);
        }

        public void AddProvider(ICustomProvider provider)
        {
            var adaptedRootProvider = new CustomProviderAdapter(provider, typeof(TRoot));
            _context.ValueProvider.AddProvider(adaptedRootProvider);
        }

        private RootProviderBuilder<TRoot> Add(MatchingProvider provider)
        {
            _providers.Add(provider);
            return this;
        }

        public IRootProviderBuilder<TRoot> With<T>(T value) => Add(_innerBuilder.With(value));
        public IRootProviderBuilder<TRoot> With<T>(Func<T> func) => Add(_innerBuilder.With(func));
        public IRootProviderBuilder<TRoot> With<T>(T value, string name) => Add(_innerBuilder.With(value, name));
        public IRootProviderBuilder<TRoot> With<T>(Func<T> func, string name) => Add(_innerBuilder.With(func, name));

        public IRootProviderBuilder<TRoot> WithParameter<T>(T value) => Add(_innerBuilder.WithParameter(value));
        public IRootProviderBuilder<TRoot> WithParameter<T>(Func<T> func) => Add(_innerBuilder.WithParameter(func));
        public IRootProviderBuilder<TRoot> WithParameter<T>(T value, string name) => Add(_innerBuilder.WithParameter(value, name));
        public IRootProviderBuilder<TRoot> WithParameter<T>(Func<T> func, string name) => Add(_innerBuilder.WithParameter(func, name));

        public IRootProviderBuilder<TRoot> WithPropertyOrField<T>(T value) => Add(_innerBuilder.WithPropertyOrField(value));
        public IRootProviderBuilder<TRoot> WithPropertyOrField<T>(Func<T> func) => Add(_innerBuilder.WithPropertyOrField(func));
        public IRootProviderBuilder<TRoot> WithPropertyOrField<T>(T value, string name) => Add(_innerBuilder.WithPropertyOrField(value, name));
        public IRootProviderBuilder<TRoot> WithPropertyOrField<T>(Func<T> func, string name) => Add(_innerBuilder.WithPropertyOrField(func, name));
    }
}
