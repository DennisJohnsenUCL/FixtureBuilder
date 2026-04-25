using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories;
using FixtureBuilder.FixtureFactories.WithMatching;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.WithMatching
{
    internal sealed class RootProviderBuilderTests
    {
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IFixtureContext>();
        }

        private RootProviderBuilder<string> CreateBuilder() => new(_contextMock.Object);

        [Test]
        public void With_ReturnsBuilderForChaining()
        {
            var valueProviderMock = new Mock<ICompositeValueProvider>();
            _contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);
            var builder = CreateBuilder();

            var result = builder.With(42);

            Assert.That(result, Is.SameAs(builder));
        }

        [Test]
        public void With_AddsProviderToContext()
        {
            var valueProviderMock = new Mock<ICompositeValueProvider>();
            _contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);
            var builder = CreateBuilder();

            builder.With(42);

            valueProviderMock.Verify(v => v.AddProvider(It.IsAny<MatchingProvider>()), Times.Once);
        }

        // Options setter

        [Test]
        public void OptionsSetter_CallsAddRootOptionsOnContext()
        {
            var builder = CreateBuilder();
            var options = new FixtureOptions();

            builder.Options = options;

            _contextMock.Verify(c => c.AddRootOptions(typeof(string), options), Times.Once);
        }

        // SetOptions

        [Test]
        public void SetOptions_ClonesBaseOptionsAndAppliesAction()
        {
            var baseOptions = new FixtureOptions { AllowPrivateConstructors = true };
            _contextMock.Setup(c => c.GetBaseOptions()).Returns(baseOptions);
            var builder = CreateBuilder();

            builder.SetOptions(o => o.AllowPrivateConstructors = false);

            _contextMock.Verify(c => c.AddRootOptions(typeof(string),
                It.Is<FixtureOptions>(o => o.AllowPrivateConstructors == false)), Times.Once);
        }

        [Test]
        public void SetOptions_DoesNotMutateBaseOptions()
        {
            var baseOptions = new FixtureOptions { AllowPrivateConstructors = true };
            _contextMock.Setup(c => c.GetBaseOptions()).Returns(baseOptions);
            var builder = CreateBuilder();

            builder.SetOptions(o => o.AllowPrivateConstructors = false);

            Assert.That(baseOptions.AllowPrivateConstructors, Is.True);
        }

        // AddProvider

        [Test]
        public void AddProvider_NullProvider_Throws()
        {
            var builder = CreateBuilder();

            Assert.Throws<ArgumentNullException>(() => builder.AddProvider(null!));
        }

        [Test]
        public void AddProvider_AddsAdaptedProviderToContext()
        {
            var valueProviderMock = new Mock<ICompositeValueProvider>();
            _contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);
            var builder = CreateBuilder();
            var providerMock = new Mock<ICustomProvider>();

            builder.AddProvider(providerMock.Object);

            valueProviderMock.Verify(v => v.AddProvider(It.IsAny<CustomProviderAdapter>()), Times.Once);
        }

        [Test]
        public void AddConverter_NullConverter_Throws()
        {
            var builder = CreateBuilder();

            Assert.Throws<ArgumentNullException>(() => builder.AddConverter(null!));
        }

        [Test]
        public void AddConverter_AddsAdaptedConverterToContext()
        {
            var compositeMock = new Mock<ICompositeConverter>();
            var converterGraph = new ConverterGraph(Mock.Of<IValueConverter>(), compositeMock.Object);
            _contextMock.Setup(c => c.Converter).Returns(converterGraph);
            var builder = CreateBuilder();
            var converterMock = new Mock<ICustomConverter>();

            builder.AddConverter(converterMock.Object);

            compositeMock.Verify(v => v.AddConverter(It.IsAny<CustomConverterAdapter>()), Times.Once);
        }
    }
}
