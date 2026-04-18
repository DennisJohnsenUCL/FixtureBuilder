using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.FixtureFactories.WithMatching
{
    internal class MatchingProviderBuilder : IProviderBuilder<MatchingProvider>
    {
        public MatchingProvider With<T>(T value)
        {
            return Build([new TypeRule(typeof(T))], value);
        }

        public MatchingProvider With<T>(Func<T> func)
        {
            return Build([new TypeRule(typeof(T))], func);
        }

        public MatchingProvider With<T>(T value, string name)
        {
            return Build([new TypeRule(typeof(T)), new NameRule(name)], value);
        }

        public MatchingProvider With<T>(Func<T> func, string name)
        {
            return Build([new TypeRule(typeof(T)), new NameRule(name)], func);
        }

        public MatchingProvider WithParameter<T>(T value)
        {
            return Build([new ParameterRule(), new TypeRule(typeof(T))], value);
        }

        public MatchingProvider WithParameter<T>(Func<T> func)
        {
            return Build([new ParameterRule(), new TypeRule(typeof(T))], func);
        }

        public MatchingProvider WithParameter<T>(T value, string name)
        {
            return Build([new ParameterRule(), new TypeRule(typeof(T)), new NameRule(name)], value);
        }

        public MatchingProvider WithParameter<T>(Func<T> func, string name)
        {
            return Build([new ParameterRule(), new TypeRule(typeof(T)), new NameRule(name)], func);
        }

        public MatchingProvider WithPropertyOrField<T>(T value)
        {
            return Build([new DataMemberRule(), new TypeRule(typeof(T))], value);
        }

        public MatchingProvider WithPropertyOrField<T>(Func<T> func)
        {
            return Build([new DataMemberRule(), new TypeRule(typeof(T))], func);
        }

        public MatchingProvider WithPropertyOrField<T>(T value, string name)
        {
            return Build([new DataMemberRule(), new TypeRule(typeof(T)), new NameRule(name)], value);
        }

        public MatchingProvider WithPropertyOrField<T>(Func<T> func, string name)
        {
            return Build([new DataMemberRule(), new TypeRule(typeof(T)), new NameRule(name)], func);
        }

        private static MatchingProvider Build<T>(IWithRule[] rules, T value)
            => new(rules, value);

        private static MatchingProvider Build<T>(IWithRule[] rules, Func<T> func)
            => new(rules, () => func());
    }
}
