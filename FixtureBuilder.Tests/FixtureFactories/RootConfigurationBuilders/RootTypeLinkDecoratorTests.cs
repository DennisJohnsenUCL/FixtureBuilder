using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.FixtureFactories.RootConfigurationBuilders;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.RootConfigurationBuilders
{
    internal sealed class RootTypeLinkDecoratorTests
    {
        [Test]
        public void Constructor_NullTypeLink_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new RootTypeLinkDecorator(null!, typeof(StringConstructorClass)));
        }

        [Test]
        public void Constructor_NullRootType_ThrowsArgumentNullException()
        {
            var typeLink = Mock.Of<ITypeLink>();

            Assert.Throws<ArgumentNullException>(() => new RootTypeLinkDecorator(typeLink, null!));
        }
    }
}
