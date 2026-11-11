using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching.WithRules
{
    internal sealed class RootTypeRuleTests
    {
        [Test]
        public void Constructor_NullRootType_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new RootTypeRule(null!));
        }
    }
}
