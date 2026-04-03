using FixtureBuilder.FixtureContexts;
using FixtureBuilder.FixtureProviders;
using Moq;

namespace FixtureBuilder.Tests.FixtureProviders
{
    internal class CompositeFixtureProviderTests
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
            Assert.Throws<ArgumentNullException>(() => new CompositeFixtureProvider(null!));
        }

        [Test]
        public void Resolve_WhenFirstProviderReturnsNonNull_ReturnsItsResult()
        {
            var expected = "from-first";
            var first = CreateProvider(expected);
            var second = CreateProvider("from-second");
            var sut = new CompositeFixtureProvider([first.Object, second.Object]);

            var result = sut.Resolve(_request, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
            second.Verify(p => p.Resolve(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void Resolve_WhenFirstReturnsNull_TriesSecondProvider()
        {
            var expected = "from-second";
            var first = CreateProvider(null);
            var second = CreateProvider(expected);
            var sut = new CompositeFixtureProvider([first.Object, second.Object]);

            var result = sut.Resolve(_request, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void Resolve_WhenAllProvidersReturnNull_ReturnsNull()
        {
            var first = CreateProvider(null);
            var second = CreateProvider(null);
            var sut = new CompositeFixtureProvider([first.Object, second.Object]);

            var result = sut.Resolve(_request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_WhenNoProviders_ReturnsNull()
        {
            var sut = new CompositeFixtureProvider([]);

            var result = sut.Resolve(_request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_PassesCorrectArgumentsToProviders()
        {
            var provider = new Mock<IFixtureProvider>();
            provider.Setup(p => p.Resolve(_request, _contextMock.Object)).Returns("result");
            var sut = new CompositeFixtureProvider([provider.Object]);

            sut.Resolve(_request, _contextMock.Object);

            provider.Verify(p => p.Resolve(_request, _contextMock.Object), Times.Once);
        }

        private Mock<IFixtureProvider> CreateProvider(object? returnValue)
        {
            var mock = new Mock<IFixtureProvider>();
            mock.Setup(p => p.Resolve(_request, _contextMock.Object)).Returns(returnValue);
            return mock;
        }
    }
}
