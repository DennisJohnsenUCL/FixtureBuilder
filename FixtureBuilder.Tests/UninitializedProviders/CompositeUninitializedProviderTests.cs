using FixtureBuilder.FixtureContexts;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.ValueProviders.Providers;
using Moq;

namespace FixtureBuilder.Tests.UninitializedProviders
{
    internal class CompositeUninitializedProviderTests
    {
        private Mock<IFixtureContext> _contextMock;
        private DefaultBclTypeProvider _defaultBclTypeProvider;
        private FixtureRequest _request;
        private const InitializeMembers DefaultInitializeMembers = InitializeMembers.All;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IFixtureContext>();
            _defaultBclTypeProvider = new DefaultBclTypeProvider();
            _request = new FixtureRequest(typeof(string));
        }

        [Test]
        public void Constructor_WhenDefaultBclTypeProviderIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CompositeUninitializedProvider(null!));
        }

        [Test]
        public void ResolveUninitialized_WhenContextResolveReturnsNonNull_ReturnsItsResult()
        {
            var expected = "from-context-resolve";
            _contextMock.Setup(c => c.Resolve(_request, _contextMock.Object)).Returns(expected);
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
            _contextMock.Verify(c => c.ResolveUninitialized(It.IsAny<FixtureRequest>(), It.IsAny<InitializeMembers>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void ResolveUninitialized_WhenContextResolveReturnsNull_AndBclProviderResolves_ReturnsBclResult()
        {
            var bclRequest = new FixtureRequest(typeof(List<int>));
            _contextMock.Setup(c => c.Resolve(bclRequest, _contextMock.Object)).Returns((object?)null);
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(bclRequest, DefaultInitializeMembers, _contextMock.Object);

            Assert.That(result, Is.TypeOf<List<int>>());
            _contextMock.Verify(c => c.ResolveUninitialized(It.IsAny<FixtureRequest>(), It.IsAny<InitializeMembers>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void ResolveUninitialized_WhenContextResolveAndBclBothReturnNull_FallsBackToResolveUninitialized()
        {
            var nonSystemRequest = new FixtureRequest(typeof(CompositeUninitializedProvider));
            var expected = new CompositeUninitializedProvider(_defaultBclTypeProvider);
            _contextMock.Setup(c => c.Resolve(nonSystemRequest, _contextMock.Object)).Returns((object?)null);
            _contextMock.Setup(c => c.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object))
                .Returns(expected);
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object);

            Assert.That(result, Is.SameAs(expected));
            _contextMock.Verify(c => c.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object), Times.Once);
        }

        [Test]
        public void ResolveUninitialized_WhenAllReturnNull_ReturnsNull()
        {
            var nonSystemRequest = new FixtureRequest(typeof(CompositeUninitializedProvider));
            _contextMock.Setup(c => c.Resolve(nonSystemRequest, _contextMock.Object)).Returns((object?)null);
            _contextMock.Setup(c => c.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object))
                .Returns((object?)null);
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveUninitialized_PassesCorrectArgumentsToContextResolve()
        {
            _contextMock.Setup(c => c.Resolve(_request, _contextMock.Object)).Returns("result");
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            sut.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object);

            _contextMock.Verify(c => c.Resolve(_request, _contextMock.Object), Times.Once);
        }
    }
}
