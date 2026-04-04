using FixtureBuilder.FixtureContexts;
using FixtureBuilder.ValueProviders;
using Moq;

namespace FixtureBuilder.Tests.ValueProviders
{
    internal class CompositeValueProviderTests
    {
        private Mock<IFixtureContext> _contextMock;
        private FixtureRequest _request;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IFixtureContext>();
            _request = new FixtureRequest(typeof(string));
        }

        [Test]
        public void Constructor_NullProviders_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CompositeValueProvider(null!));
        }

        [Test]
        public void Resolve_WhenFirstProviderReturnsNonNull_ReturnsItsResult()
        {
            var expected = "from-first";
            var first = CreateProvider(expected);
            var second = CreateProvider("from-second");
            var sut = new CompositeValueProvider([first.Object, second.Object]);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
            second.Verify(p => p.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void Resolve_WhenFirstReturnsNull_TriesSecondProvider()
        {
            var expected = "from-second";
            var first = CreateProvider(null);
            var second = CreateProvider(expected);
            var sut = new CompositeValueProvider([first.Object, second.Object]);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void Resolve_WhenAllProvidersReturnNull_ReturnsNull()
        {
            var first = CreateProvider(null);
            var second = CreateProvider(null);
            var sut = new CompositeValueProvider([first.Object, second.Object]);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_WhenNoProviders_ReturnsNull()
        {
            var sut = new CompositeValueProvider([]);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_PassesCorrectArgumentsToProviders()
        {
            var provider = new Mock<IValueProvider>();
            provider.Setup(p => p.ResolveValue(_request, _contextMock.Object)).Returns("result");
            var sut = new CompositeValueProvider([provider.Object]);

            sut.ResolveValue(_request, _contextMock.Object);

            provider.Verify(p => p.ResolveValue(_request, _contextMock.Object), Times.Once);
        }

        private Mock<IValueProvider> CreateProvider(object? returnValue)
        {
            var mock = new Mock<IValueProvider>();
            mock.Setup(p => p.ResolveValue(_request, _contextMock.Object)).Returns(returnValue);
            return mock;
        }
    }
}
