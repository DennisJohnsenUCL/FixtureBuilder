using FixtureBuilder.ValueProviders;
using FixtureBuilder.ValueProviders.ValueProviderBuilders;

namespace FixtureBuilder.Tests.ValueProviders.ValueProviderBuilders
{
    internal class ValueProviderFactoryTests
    {
        private ValueProviderFactory _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new ValueProviderFactory();
        }

        [Test]
        public void Constructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new ValueProviderFactory());
        }

        [Test]
        public void CreateDefaultValueProvider_ReturnsNonNullResult()
        {
            var result = _sut.CreateDefaultValueProvider();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CreateDefaultValueProvider_ReturnsCompositeValueProvider()
        {
            var result = _sut.CreateDefaultValueProvider();

            Assert.That(result, Is.InstanceOf<CompositeValueProvider>());
        }
    }
}
