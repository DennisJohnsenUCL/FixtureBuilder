using FixtureBuilder.FixtureFactories.WithMatching;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching
{
    internal sealed class MatchingProviderTests
    {
        [Test]
        public void Constructor_NullRules_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MatchingProvider(null!, "value"));
        }
    }
}
