using FixtureBuilder.FixtureProviders;
using FixtureBuilder.FixtureProviders.FixtureProviderBuilders;

namespace FixtureBuilder.Tests.FixtureProviders.FixtureProviderBuilders
{
    internal class FixtureProviderFactoryTests
    {
        private FixtureProviderFactory _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new FixtureProviderFactory();
        }

        [Test]
        public void Constructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new FixtureProviderFactory());
        }

        [Test]
        public void CreateDefaultFixtureProvider_ReturnsNonNullResult()
        {
            var result = _sut.CreateDefaultFixtureProvider();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CreateDefaultFixtureProvider_ReturnsCompositeFixtureProvider()
        {
            var result = _sut.CreateDefaultFixtureProvider();

            Assert.That(result, Is.InstanceOf<CompositeFixtureProvider>());
        }
    }
}
