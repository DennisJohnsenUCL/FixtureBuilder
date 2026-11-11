using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching.WithRules
{
    internal sealed class TypeRuleTests
    {
        [Test]
        public void Constructor_NullType_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TypeRule(null!));
        }
    }
}
