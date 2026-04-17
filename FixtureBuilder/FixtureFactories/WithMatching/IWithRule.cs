using FixtureBuilder.Core;

namespace FixtureBuilder.FixtureFactories.WithMatching
{
    internal interface IWithRule
    {
        public bool IsMatch(FixtureRequest request);
    }
}
