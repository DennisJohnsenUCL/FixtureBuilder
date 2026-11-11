using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.FixtureFactories.RootConfigurationBuilders;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.RootConfigurationBuilders
{
    internal sealed class RootConverterDecoratorTests
    {
        [Test]
        public void Constructor_NullConverter_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new RootConverterDecorator(null!, typeof(StringConstructorClass)));
        }

        [Test]
        public void Constructor_NullRootType_ThrowsArgumentNullException()
        {
            var converter = Mock.Of<IValueConverter>();

            Assert.Throws<ArgumentNullException>(() => new RootConverterDecorator(converter, null!));
        }
    }
}
