using FixtureBuilder.FixtureContexts;
using FixtureBuilder.ValueProviders;
using Moq;

namespace FixtureBuilder.Tests.ValueProviders
{
    internal class CompositeValueProviderTests
    {
        private Mock<IFixtureContext> _contextMock;
        private FixtureRequest _request;

        private Mock<IValueProvider> CreateProvider(object? returnValue)
        {
            var mock = new Mock<IValueProvider>();
            mock.Setup(p => p.ResolveValue(_request, _contextMock.Object)).Returns(returnValue);
            return mock;
        }

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
        public void Resolve_WhenFirstReturnsNoResult_TriesSecondProvider()
        {
            var expected = "from-second";
            var first = CreateProvider(new NoResult());
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
        public void Resolve_WhenNoProviders_ReturnsNoResult()
        {
            var sut = new CompositeValueProvider([]);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
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

        [Test]
        public void Resolve_WhenProviderReturnsNullForNonNullableValueType_ThrowsInvalidOperationException()
        {
            var request = new FixtureRequest(typeof(int));
            var provider = new Mock<IValueProvider>();
            provider.Setup(p => p.ResolveValue(request, _contextMock.Object)).Returns(null!);
            var sut = new CompositeValueProvider([provider.Object]);

            Assert.Throws<InvalidOperationException>(
                () => sut.ResolveValue(request, _contextMock.Object));
        }

        [Test]
        public void Resolve_WhenProviderReturnsNullForNullableValueType_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(int?));
            var provider = new Mock<IValueProvider>();
            provider.Setup(p => p.ResolveValue(request, _contextMock.Object)).Returns(null!);
            var sut = new CompositeValueProvider([provider.Object]);

            var result = sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }
    }
}
