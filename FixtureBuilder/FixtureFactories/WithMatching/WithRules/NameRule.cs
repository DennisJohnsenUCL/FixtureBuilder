using FixtureBuilder.Core;

namespace FixtureBuilder.FixtureFactories.WithMatching.WithRules
{
    internal class NameRule : IWithRule
    {
        private readonly string _name;

        public NameRule(string name)
        {
            _name = name;
        }

        public bool IsMatch(FixtureRequest request)
        {
            return request.Name == _name;
        }
    }
}
