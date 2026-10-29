using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Creation.UninitializedProviders
{
    internal class CompositeUninitializedProviderTests
    {
        private Mock<IFixtureContext> _contextMock;
        private Mock<ICompositeValueProvider> _valueProviderMock;
        private Mock<IUninitializedProvider> _uninitializedProviderMock;
        private DefaultBclTypeProvider _defaultBclTypeProvider;
        private RecursiveResolveContext _recursiveResolveContext;
        private FixtureRequest _request;
        private const InitializeMembers _defaultInitializeMembers = InitializeMembers.All;

        [SetUp]
        public void SetUp()
        {
            _valueProviderMock = new Mock<ICompositeValueProvider>();
            _uninitializedProviderMock = new Mock<IUninitializedProvider>();
            _contextMock = new Mock<IFixtureContext>();
            _contextMock.Setup(c => c.ValueProvider).Returns(_valueProviderMock.Object);
            _contextMock.Setup(c => c.UninitializedProvider).Returns(_uninitializedProviderMock.Object);
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
            _valueProviderMock.Setup(c => c.ResolveValue(_request, _contextMock.Object)).Returns(expected);
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(_request, _defaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.SameAs(expected));
            _uninitializedProviderMock.Verify(c => c.ResolveUninitialized(It.IsAny<FixtureRequest>(), It.IsAny<InitializeMembers>(), It.IsAny<IFixtureContext>(), It.IsAny<RecursiveResolveContext>()), Times.Never);
        }

        [Test]
        public void ResolveUninitialized_WhenContextResolveReturnsNoResult_AndBclProviderResolves_ReturnsBclResult()
        {
            var bclRequest = new FixtureRequest(typeof(List<int>));
            _valueProviderMock.Setup(c => c.ResolveValue(bclRequest, _contextMock.Object)).Returns(NoResult.Instance);
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(bclRequest, _defaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.TypeOf<List<int>>());
            _uninitializedProviderMock.Verify(c => c.ResolveUninitialized(It.IsAny<FixtureRequest>(), It.IsAny<InitializeMembers>(), It.IsAny<IFixtureContext>(), It.IsAny<RecursiveResolveContext>()), Times.Never);
        }

        [Test]
        public void ResolveUninitialized_WhenContextResolveAndBclBothReturnNoResult_FallsBackToResolveUninitialized()
        {
            var nonSystemRequest = new FixtureRequest(typeof(CompositeUninitializedProvider));
            var expected = new CompositeUninitializedProvider(_defaultBclTypeProvider);
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.OptionsFor(It.IsAny<Type>())).Returns(options);
            _valueProviderMock.Setup(c => c.ResolveValue(nonSystemRequest, _contextMock.Object)).Returns(NoResult.Instance);
            _uninitializedProviderMock.Setup(c => c.ResolveUninitialized(nonSystemRequest, _defaultInitializeMembers, _contextMock.Object, _recursiveResolveContext))
                .Returns(expected);
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(nonSystemRequest, _defaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.SameAs(expected));
            _uninitializedProviderMock.Verify(c => c.ResolveUninitialized(nonSystemRequest, _defaultInitializeMembers, _contextMock.Object, _recursiveResolveContext), Times.Once);
        }

        [Test]
        public void ResolveUninitialized_NoResultReturnAllowed_WhenAllReturnNoResult_ReturnsNoResult()
        {
            var nonSystemRequest = new FixtureRequest(typeof(CompositeUninitializedProvider));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.OptionsFor(It.IsAny<Type>())).Returns(options);
            _valueProviderMock.Setup(c => c.ResolveValue(nonSystemRequest, _contextMock.Object)).Returns(NoResult.Instance);
            _uninitializedProviderMock.Setup(c => c.ResolveUninitialized(nonSystemRequest, _defaultInitializeMembers, _contextMock.Object, _recursiveResolveContext))
                .Returns(NoResult.Instance);
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(nonSystemRequest, _defaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void ResolveUninitialized_NoResultReturnNotAllowed_WhenAllReturnNoResult_ThrowsException()
        {
            var nonSystemRequest = new FixtureRequest(typeof(CompositeUninitializedProvider));
            var options = new FixtureOptions() { AllowSkipUninitializableMembers = false };
            _contextMock.Setup(c => c.OptionsFor(It.IsAny<Type>())).Returns(options);
            _valueProviderMock.Setup(c => c.ResolveValue(nonSystemRequest, _contextMock.Object)).Returns(NoResult.Instance);
            _uninitializedProviderMock.Setup(c => c.ResolveUninitialized(nonSystemRequest, _defaultInitializeMembers, _contextMock.Object, _recursiveResolveContext))
                .Returns(NoResult.Instance);
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            Assert.Throws<InvalidOperationException>(() => sut.ResolveUninitialized(nonSystemRequest, _defaultInitializeMembers, _contextMock.Object, _recursiveResolveContext));
        }

        [Test]
        public void ResolveUninitialized_PassesCorrectArgumentsToContextResolve()
        {
            _valueProviderMock.Setup(c => c.ResolveValue(_request, _contextMock.Object)).Returns("result");
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            sut.ResolveUninitialized(_request, _defaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            _valueProviderMock.Verify(c => c.ResolveValue(_request, _contextMock.Object), Times.Once);
        }
    }
}
