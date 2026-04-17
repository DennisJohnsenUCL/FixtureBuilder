using FixtureBuilder.Core;

namespace FixtureBuilder.FixtureFactories.WithMatching.WithRules
{
    internal class NameRule : IWithRule
    {
        private readonly string _name;

        public NameRule(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            _name = name;
        }

        public bool IsMatch(FixtureRequest request)
        {
            if (request.Name == _name) return true;
            return false;
        }
    }
}
