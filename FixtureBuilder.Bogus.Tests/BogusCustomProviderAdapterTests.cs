using Bogus;
using FixtureBuilder.Bogus.BogusFixtureFactories;
using Moq;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureFactoryTests
{
    internal sealed class BogusCustomProviderAdapterTests
    {
        [Test]
        public void Constructor_NullProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BogusCustomProviderAdapter(null!, new Faker()));
        }

        [Test]
        public void Constructor_NullFaker_Throws()
        {
            var provider = new Mock<IBogusCustomProvider>();
            Assert.Throws<ArgumentNullException>(() => new BogusCustomProviderAdapter(provider.Object, null!));
        }
    }
}
