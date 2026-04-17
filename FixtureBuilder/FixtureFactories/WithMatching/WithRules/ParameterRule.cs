using System.Reflection;
using FixtureBuilder.Core;

namespace FixtureBuilder.FixtureFactories.WithMatching.WithRules
{
    internal class ParameterRule : IWithRule
    {
        public bool IsMatch(FixtureRequest request)
        {
            if (request.RequestSource is ParameterInfo) return true;
            return false;
        }
    }
}
