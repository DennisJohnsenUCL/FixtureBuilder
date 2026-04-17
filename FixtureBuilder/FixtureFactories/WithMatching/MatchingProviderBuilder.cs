using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.FixtureFactories.WithMatching
{
    internal class MatchingProviderBuilder : IProviderBuilder<MatchingProvider>
    {
        public MatchingProvider With<T>(T value)
        {
            return new MatchingProvider([new TypeRule(typeof(T))], value);
        }

        public MatchingProvider With<T>(T value, string name)
        {
            return new MatchingProvider([new TypeRule(typeof(T)), new NameRule(name)], value);
        }

        public MatchingProvider WithParameter<T>(T value)
        {
            return new MatchingProvider([new ParameterRule(), new TypeRule(typeof(T))], value);
        }

        public MatchingProvider WithParameter<T>(T value, string name)
        {
            return new MatchingProvider([new ParameterRule(), new TypeRule(typeof(T)), new NameRule(name)], value);
        }

        public MatchingProvider WithPropertyOrField<T>(T value)
        {
            return new MatchingProvider([new DataMemberRule(), new TypeRule(typeof(T))], value);
        }

        public MatchingProvider WithPropertyOrField<T>(T value, string name)
        {
            return new MatchingProvider([new DataMemberRule(), new TypeRule(typeof(T)), new NameRule(name)], value);
        }

        public MatchingProvider WithNamed(string name, object? value)
        {
            return new MatchingProvider([new NameRule(name)], value);
        }
    }
}
