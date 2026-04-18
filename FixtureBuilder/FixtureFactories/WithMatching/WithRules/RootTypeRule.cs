using FixtureBuilder.Core;

namespace FixtureBuilder.FixtureFactories.WithMatching.WithRules
{
    internal class RootTypeRule : IWithRule
    {
        private readonly Type _rootType;

        public RootTypeRule(Type rootType)
        {
            ArgumentNullException.ThrowIfNull(rootType);
            _rootType = rootType;
        }

        public bool IsMatch(FixtureRequest request)
        {
            return request.RootType == _rootType;
        }
    }
}
