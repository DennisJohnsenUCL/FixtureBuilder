using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories.RootConfigurationBuilders;
using FixtureBuilder.FixtureFactories.WithMatching;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.RootProviderBuilders
{
    internal sealed class RootProviderBuilderTests
    {
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IFixtureContext>();
        }

        private RootConfigurationBuilder<string> CreateBuilder() => new(_contextMock.Object);

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

            valueProviderMock.Verify(v => v.AddRootProvider(It.IsAny<MatchingProvider>()), Times.Once);
        }

        // Options setter

        [Test]
        public void OptionsSetter_CallsSetRootOptionsOnContext()
        {
            var builder = CreateBuilder();
            var options = new FixtureOptions();

            builder.Options = options;

            _contextMock.Verify(c => c.SetRootOptions(typeof(string), options), Times.Once);
        }

        [Test]
        public void OptionsSetter_NullOptions_Throws()
        {
            var builder = CreateBuilder();

            Assert.Throws<ArgumentNullException>(() => builder.Options = null!);
        }

        // SetOptions

        [Test]
        public void SetOptions_ClonesCurrentOptionsAndAppliesAction()
        {
            var currentOptions = new FixtureOptions { AllowPrivateConstructors = true, PreferSimplestConstructor = true };
            _contextMock.Setup(c => c.OptionsFor(typeof(string))).Returns(currentOptions);
            var builder = CreateBuilder();

            builder.SetOptions(o => o.AllowPrivateConstructors = false);

            _contextMock.Verify(c => c.SetRootOptions(typeof(string),
                It.Is<FixtureOptions>(o => o.AllowPrivateConstructors == false && o.PreferSimplestConstructor)), Times.Once);
        }

        [Test]
        public void SetOptions_DoesNotMutateCurrentOptions()
        {
            var currentOptions = new FixtureOptions { AllowPrivateConstructors = true };
            _contextMock.Setup(c => c.OptionsFor(typeof(string))).Returns(currentOptions);
            var builder = CreateBuilder();

            builder.SetOptions(o => o.AllowPrivateConstructors = false);

            Assert.That(currentOptions.AllowPrivateConstructors, Is.True);
        }

        [Test]
        public void SetOptions_NullAction_Throws()
        {
            var builder = CreateBuilder();

            Assert.Throws<ArgumentNullException>(() => builder.SetOptions(null!));
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

            valueProviderMock.Verify(v => v.AddRootProvider(It.IsAny<RootProviderDecorator>()), Times.Once);
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

            compositeMock.Verify(v => v.AddConverter(It.IsAny<RootConverterDecorator>()), Times.Once);
        }

        // AddTypeLink

        [Test]
        public void AddTypeLink_NullTypeLink_Throws()
        {
            var builder = CreateBuilder();

            Assert.Throws<ArgumentNullException>(() => builder.AddTypeLink((ICustomTypeLink)null!));
        }

        [Test]
        public void AddTypeLink_AddsAdaptedTypeLinkToContext()
        {
            var compositeTypeLinkMock = new Mock<ICompositeTypeLink>();
            _contextMock.Setup(c => c.TypeLink).Returns(compositeTypeLinkMock.Object);
            var builder = CreateBuilder();
            var typeLinkMock = new Mock<ICustomTypeLink>();

            builder.AddTypeLink(typeLinkMock.Object);

            compositeTypeLinkMock.Verify(v => v.AddRootTypeLink(It.IsAny<RootTypeLinkDecorator>()), Times.Once);
        }
    }
}
