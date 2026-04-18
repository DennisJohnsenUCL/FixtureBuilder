using System.Reflection;
using FixtureBuilder.Core;

namespace FixtureBuilder.FixtureFactories.WithMatching.WithRules
{
    internal class ParameterRule : IWithRule
    {
        public bool IsMatch(FixtureRequest request)
        {
            return request.RequestSource is ParameterInfo;
        }
    }
}
