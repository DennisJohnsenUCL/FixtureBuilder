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
        private RecursiveResolveContext _recursiveResolveContext;
        private FixtureRequest _request;
        private const InitializeMembers DefaultInitializeMembers = InitializeMembers.All;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IFixtureContext>();
            _defaultBclTypeProvider = new DefaultBclTypeProvider();
            _recursiveResolveContext = new();
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
            _contextMock.Setup(c => c.ResolveValue(_request, _contextMock.Object)).Returns(expected);
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.SameAs(expected));
            _contextMock.Verify(c => c.ResolveUninitialized(It.IsAny<FixtureRequest>(), It.IsAny<InitializeMembers>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void ResolveUninitialized_WhenContextResolveReturnsNoResult_AndBclProviderResolves_ReturnsBclResult()
        {
            var bclRequest = new FixtureRequest(typeof(List<int>));
            _contextMock.Setup(c => c.ResolveValue(bclRequest, _contextMock.Object)).Returns(new NoResult());
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(bclRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.TypeOf<List<int>>());
            _contextMock.Verify(c => c.ResolveUninitialized(It.IsAny<FixtureRequest>(), It.IsAny<InitializeMembers>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void ResolveUninitialized_WhenContextResolveAndBclBothReturnNoResult_FallsBackToResolveUninitialized()
        {
            var nonSystemRequest = new FixtureRequest(typeof(CompositeUninitializedProvider));
            var expected = new CompositeUninitializedProvider(_defaultBclTypeProvider);
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.ResolveValue(nonSystemRequest, _contextMock.Object)).Returns(new NoResult());
            _contextMock.Setup(c => c.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext))
                .Returns(expected);
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.SameAs(expected));
            _contextMock.Verify(c => c.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext), Times.Once);
        }

        [Test]
        public void ResolveUninitialized_NoResultReturnAllowed_WhenAllReturnNoResult_ReturnsNoResult()
        {
            var nonSystemRequest = new FixtureRequest(typeof(CompositeUninitializedProvider));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.ResolveValue(nonSystemRequest, _contextMock.Object)).Returns(new NoResult());
            _contextMock.Setup(c => c.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext))
                .Returns(new NoResult());
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void ResolveUninitialized_NoResultReturnNotAllowed_WhenAllReturnNoResult_ThrowsException()
        {
            var nonSystemRequest = new FixtureRequest(typeof(CompositeUninitializedProvider));
            var options = new FixtureOptions() { AllowSkipUninitializableMembers = false };
            _contextMock.Setup(c => c.Options).Returns(options);
            _contextMock.Setup(c => c.ResolveValue(nonSystemRequest, _contextMock.Object)).Returns(new NoResult());
            _contextMock.Setup(c => c.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext))
                .Returns(new NoResult());
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            Assert.Throws<InvalidOperationException>(() => sut.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext));
        }

        [Test]
        public void ResolveUninitialized_PassesCorrectArgumentsToContextResolve()
        {
            _contextMock.Setup(c => c.ResolveValue(_request, _contextMock.Object)).Returns("result");
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            sut.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            _contextMock.Verify(c => c.ResolveValue(_request, _contextMock.Object), Times.Once);
        }
    }
}
