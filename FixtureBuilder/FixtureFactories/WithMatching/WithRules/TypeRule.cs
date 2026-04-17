using FixtureBuilder.Core;

namespace FixtureBuilder.FixtureFactories.WithMatching.WithRules
{
    internal class TypeRule : IWithRule
    {
        private readonly Type _type;

        public TypeRule(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);
            _type = type;
        }

        public bool IsMatch(FixtureRequest request)
        {
            if (request.Type == _type) return true;
            return false;
        }
    }
}
