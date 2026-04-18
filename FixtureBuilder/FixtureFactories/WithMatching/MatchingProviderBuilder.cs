using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.FixtureFactories.WithMatching
{
    internal class MatchingProviderBuilder : IProviderBuilder<MatchingProvider>
    {
        private readonly IEnumerable<IWithRule> _baseRules = [];

        public MatchingProviderBuilder() : this([]) { }

        public MatchingProviderBuilder(IEnumerable<IWithRule> baseRules)
        {
            ArgumentNullException.ThrowIfNull(baseRules);
            _baseRules = baseRules;
        }

        private MatchingProvider Build<T>(IWithRule[] rules, T value)
            => new(_baseRules.Concat(rules), value);

        private MatchingProvider Build<T>(IWithRule[] rules, Func<T> func)
            => new(_baseRules.Concat(rules), () => func());

        public MatchingProvider With<T>(T value)
            => Build([new TypeRule(typeof(T))], value);

        public MatchingProvider With<T>(Func<T> func)
            => Build([new TypeRule(typeof(T))], func);

        public MatchingProvider With<T>(T value, string name)
            => Build([new TypeRule(typeof(T)), new NameRule(name)], value);

        public MatchingProvider With<T>(Func<T> func, string name)
            => Build([new TypeRule(typeof(T)), new NameRule(name)], func);

        public MatchingProvider WithParameter<T>(T value)
            => Build([new ParameterRule(), new TypeRule(typeof(T))], value);

        public MatchingProvider WithParameter<T>(Func<T> func)
            => Build([new ParameterRule(), new TypeRule(typeof(T))], func);

        public MatchingProvider WithParameter<T>(T value, string name)
            => Build([new ParameterRule(), new TypeRule(typeof(T)), new NameRule(name)], value);

        public MatchingProvider WithParameter<T>(Func<T> func, string name)
            => Build([new ParameterRule(), new TypeRule(typeof(T)), new NameRule(name)], func);

        public MatchingProvider WithPropertyOrField<T>(T value)
            => Build([new DataMemberRule(), new TypeRule(typeof(T))], value);

        public MatchingProvider WithPropertyOrField<T>(Func<T> func)
            => Build([new DataMemberRule(), new TypeRule(typeof(T))], func);

        public MatchingProvider WithPropertyOrField<T>(T value, string name)
            => Build([new DataMemberRule(), new TypeRule(typeof(T)), new NameRule(name)], value);

        public MatchingProvider WithPropertyOrField<T>(Func<T> func, string name)
            => Build([new DataMemberRule(), new TypeRule(typeof(T)), new NameRule(name)], func);
    }
}
