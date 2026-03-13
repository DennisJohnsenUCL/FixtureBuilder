using FixtureBuilder.FixtureContexts;
using FixtureBuilder.FixtureProviders;
using FixtureBuilder.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.UninitializedProviders
{
    internal class CompositeUninitializedProviderTests
    {
        private Mock<IFixtureContext> _contextMock;
        private FixtureRequest _request;
        private const InitializeMembers DefaultInitializeMembers = InitializeMembers.All;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IFixtureContext>();
            _request = new FixtureRequest(typeof(string));
        }

        [Test]
        public void Constructor_WhenProvidersIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CompositeUninitializedProvider(null!));
        }

        [Test]
        public void Constructor_WhenProvidersIsValid_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new CompositeUninitializedProvider([]));
        }

        [Test]
        public void ResolveUninitialized_WhenFirstProviderReturnsNonNull_ReturnsItsResult()
        {
            var expected = "from-first";
            var first = CreateProvider(expected);
            var second = CreateProvider("from-second");
            var sut = new CompositeUninitializedProvider([first.Object, second.Object]);

            var result = sut.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
            second.Verify(p => p.Resolve(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void ResolveUninitialized_WhenFirstReturnsNull_TriesSecondProvider()
        {
            var expected = "from-second";
            var first = CreateProvider(null);
            var second = CreateProvider(expected);
            var sut = new CompositeUninitializedProvider([first.Object, second.Object]);

            var result = sut.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void ResolveUninitialized_WhenAllProvidersReturnNull_FallsBackToContext()
        {
            var expected = "from-context";
            var first = CreateProvider(null);
            var second = CreateProvider(null);
            _contextMock.Setup(c => c.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object))
                .Returns(expected);
            var sut = new CompositeUninitializedProvider([first.Object, second.Object]);

            var result = sut.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
            _contextMock.Verify(c => c.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object), Times.Once);
        }

        [Test]
        public void ResolveUninitialized_WhenNoProviders_FallsBackToContext()
        {
            var expected = "from-context";
            _contextMock.Setup(c => c.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object))
                .Returns(expected);
            var sut = new CompositeUninitializedProvider([]);

            var result = sut.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void ResolveUninitialized_WhenAllProvidersAndContextReturnNull_ReturnsNull()
        {
            var first = CreateProvider(null);
            _contextMock.Setup(c => c.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object))
                .Returns((object?)null);
            var sut = new CompositeUninitializedProvider([first.Object]);

            var result = sut.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveUninitialized_PassesCorrectArgumentsToProviders()
        {
            var provider = new Mock<IFixtureProvider>();
            provider.Setup(p => p.Resolve(_request, _contextMock.Object)).Returns("result");
            var sut = new CompositeUninitializedProvider([provider.Object]);

            sut.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object);

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
