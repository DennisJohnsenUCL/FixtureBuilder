using System.Reflection;
using FixtureBuilder.Core;

namespace FixtureBuilder.FixtureFactories.WithMatching.WithRules
{
    internal class DataMemberRule : IWithRule
    {
        public bool IsMatch(FixtureRequest request)
        {
            var requestSourceType = request.RequestSource;

            if (requestSourceType is PropertyInfo
                || requestSourceType is FieldInfo) return true;
            return false;
        }
    }
}
