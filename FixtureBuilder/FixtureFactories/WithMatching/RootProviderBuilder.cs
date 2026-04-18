using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.FixtureFactories.WithMatching
{
    public class RootProviderBuilder<TRoot> : IProviderBuilder<RootProviderBuilder<TRoot>>
    {
        private readonly List<MatchingProvider> _providers = [];
        private readonly MatchingProviderBuilder _innerBuilder = new([new RootTypeRule(typeof(TRoot))]);

        internal IReadOnlyList<MatchingProvider> Providers => _providers.AsReadOnly();

        private RootProviderBuilder<TRoot> Add(MatchingProvider provider)
        {
            _providers.Add(provider);
            return this;
        }

        public RootProviderBuilder<TRoot> With<T>(T value) => Add(_innerBuilder.With(value));
        public RootProviderBuilder<TRoot> With<T>(Func<T> func) => Add(_innerBuilder.With(func));
        public RootProviderBuilder<TRoot> With<T>(T value, string name) => Add(_innerBuilder.With(value, name));
        public RootProviderBuilder<TRoot> With<T>(Func<T> func, string name) => Add(_innerBuilder.With(func, name));

        public RootProviderBuilder<TRoot> WithParameter<T>(T value) => Add(_innerBuilder.WithParameter(value));
        public RootProviderBuilder<TRoot> WithParameter<T>(Func<T> func) => Add(_innerBuilder.WithParameter(func));
        public RootProviderBuilder<TRoot> WithParameter<T>(T value, string name) => Add(_innerBuilder.WithParameter(value, name));
        public RootProviderBuilder<TRoot> WithParameter<T>(Func<T> func, string name) => Add(_innerBuilder.WithParameter(func, name));

        public RootProviderBuilder<TRoot> WithPropertyOrField<T>(T value) => Add(_innerBuilder.WithPropertyOrField(value));
        public RootProviderBuilder<TRoot> WithPropertyOrField<T>(Func<T> func) => Add(_innerBuilder.WithPropertyOrField(func));
        public RootProviderBuilder<TRoot> WithPropertyOrField<T>(T value, string name) => Add(_innerBuilder.WithPropertyOrField(value, name));
        public RootProviderBuilder<TRoot> WithPropertyOrField<T>(Func<T> func, string name) => Add(_innerBuilder.WithPropertyOrField(func, name));
    }
}
