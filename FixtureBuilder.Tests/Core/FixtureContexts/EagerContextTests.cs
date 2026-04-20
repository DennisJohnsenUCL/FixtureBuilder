using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts
{
    internal sealed class EagerContextTests
    {
        private FixtureOptions _options;
        private ConverterGraph _converter;
        private Mock<ICompositeTypeLink> _typeLinkMock;
        private Mock<IUninitializedProvider> _uninitializedMock;
        private Mock<ICompositeValueProvider> _valueProviderMock;
        private Mock<IAutoConstructingProvider> _autoConstructingMock;
        private Mock<IConstructingProvider> _constructingMock;

        [SetUp]
        public void SetUp()
        {
            _options = new FixtureOptions();
            _converter = new ConverterGraph(new Mock<IValueConverter>().Object, new Mock<ICompositeConverter>().Object);
            _typeLinkMock = new Mock<ICompositeTypeLink>();
            _uninitializedMock = new Mock<IUninitializedProvider>();
            _valueProviderMock = new Mock<ICompositeValueProvider>();
            _autoConstructingMock = new Mock<IAutoConstructingProvider>();
            _constructingMock = new Mock<IConstructingProvider>();
        }

        [Test]
        public void Constructor_NullOptions_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new EagerContext(null!, _converter, _typeLinkMock.Object, _uninitializedMock.Object, _valueProviderMock.Object, _autoConstructingMock.Object, _constructingMock.Object));
        }

        [Test]
        public void Constructor_NullConverter_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new EagerContext(_options, null!, _typeLinkMock.Object, _uninitializedMock.Object, _valueProviderMock.Object, _autoConstructingMock.Object, _constructingMock.Object));
        }

        [Test]
        public void Constructor_NullTypeLink_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new EagerContext(_options, _converter, null!, _uninitializedMock.Object, _valueProviderMock.Object, _autoConstructingMock.Object, _constructingMock.Object));
        }

        [Test]
        public void Constructor_NullUninitializedProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new EagerContext(_options, _converter, _typeLinkMock.Object, null!, _valueProviderMock.Object, _autoConstructingMock.Object, _constructingMock.Object));
        }

        [Test]
        public void Constructor_NullValueProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new EagerContext(_options, _converter, _typeLinkMock.Object, _uninitializedMock.Object, null!, _autoConstructingMock.Object, _constructingMock.Object));
        }

        [Test]
        public void Constructor_NullAutoConstructingProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new EagerContext(_options, _converter, _typeLinkMock.Object, _uninitializedMock.Object, _valueProviderMock.Object, null!, _constructingMock.Object));
        }

        [Test]
        public void Constructor_NullConstructingProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new EagerContext(_options, _converter, _typeLinkMock.Object, _uninitializedMock.Object, _valueProviderMock.Object, _autoConstructingMock.Object, null!));
        }
    }
}
