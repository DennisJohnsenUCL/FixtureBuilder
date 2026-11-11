using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.FixtureFactories.RootConfigurationBuilders;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.RootConfigurationBuilders
{
    internal sealed class RootProviderDecoratorTests
    {
        [Test]
        public void Constructor_NullProvider_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new RootProviderDecorator(null!, typeof(StringConstructorClass)));
        }

        [Test]
        public void Constructor_NullRootType_ThrowsArgumentNullException()
        {
            var provider = Mock.Of<IValueProvider>();

            Assert.Throws<ArgumentNullException>(() => new RootProviderDecorator(provider, null!));
        }
    }
}
