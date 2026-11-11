using FixtureBuilder.Bogus.BogusFixtureFactories.Customizations;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactories
{
    internal sealed class BogusCustomProviderAdapterTests
    {
        [Test]
        public void Constructor_NullFaker_ThrowsArgumentNullException()
        {
            var provider = new FakerLocaleProvider();

            Assert.Throws<ArgumentNullException>(() => new BogusCustomProviderAdapter(provider, null!));
        }
    }
}
