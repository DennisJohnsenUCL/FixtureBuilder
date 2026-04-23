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
        private const InitializeMembers DefaultInitializeMembers = InitializeMembers.All;

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
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(string))).Returns(typeof(string));
            _valueProviderMock.Setup(c => c.ResolveValue(_request, _contextMock.Object)).Returns(expected);
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.SameAs(expected));
            _uninitializedProviderMock.Verify(c => c.ResolveUninitialized(It.IsAny<FixtureRequest>(), It.IsAny<InitializeMembers>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void ResolveUninitialized_WhenContextResolveReturnsNoResult_AndBclProviderResolves_ReturnsBclResult()
        {
            var bclRequest = new FixtureRequest(typeof(List<int>));
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(List<int>))).Returns(typeof(List<int>));
            _valueProviderMock.Setup(c => c.ResolveValue(bclRequest, _contextMock.Object)).Returns(new NoResult());
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(bclRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.TypeOf<List<int>>());
            _uninitializedProviderMock.Verify(c => c.ResolveUninitialized(It.IsAny<FixtureRequest>(), It.IsAny<InitializeMembers>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void ResolveUninitialized_WhenContextResolveAndBclBothReturnNoResult_FallsBackToResolveUninitialized()
        {
            var nonSystemRequest = new FixtureRequest(typeof(CompositeUninitializedProvider));
            var expected = new CompositeUninitializedProvider(_defaultBclTypeProvider);
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.OptionsFor(It.IsAny<Type>())).Returns(options);
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(CompositeUninitializedProvider))).Returns(typeof(CompositeUninitializedProvider));
            _valueProviderMock.Setup(c => c.ResolveValue(nonSystemRequest, _contextMock.Object)).Returns(new NoResult());
            _uninitializedProviderMock.Setup(c => c.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext))
                .Returns(expected);
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.SameAs(expected));
            _uninitializedProviderMock.Verify(c => c.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext), Times.Once);
        }

        [Test]
        public void ResolveUninitialized_NoResultReturnAllowed_WhenAllReturnNoResult_ReturnsNoResult()
        {
            var nonSystemRequest = new FixtureRequest(typeof(CompositeUninitializedProvider));
            var options = new FixtureOptions();
            _contextMock.Setup(c => c.OptionsFor(It.IsAny<Type>())).Returns(options);
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(CompositeUninitializedProvider))).Returns(typeof(CompositeUninitializedProvider));
            _valueProviderMock.Setup(c => c.ResolveValue(nonSystemRequest, _contextMock.Object)).Returns(new NoResult());
            _uninitializedProviderMock.Setup(c => c.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext))
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
            _contextMock.Setup(c => c.OptionsFor(It.IsAny<Type>())).Returns(options);
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(CompositeUninitializedProvider))).Returns(typeof(CompositeUninitializedProvider));
            _valueProviderMock.Setup(c => c.ResolveValue(nonSystemRequest, _contextMock.Object)).Returns(new NoResult());
            _uninitializedProviderMock.Setup(c => c.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext))
                .Returns(new NoResult());
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            Assert.Throws<InvalidOperationException>(() => sut.ResolveUninitialized(nonSystemRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext));
        }

        [Test]
        public void ResolveUninitialized_PassesCorrectArgumentsToContextResolve()
        {
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(string))).Returns(typeof(string));
            _valueProviderMock.Setup(c => c.ResolveValue(_request, _contextMock.Object)).Returns("result");
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            sut.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            _valueProviderMock.Verify(c => c.ResolveValue(_request, _contextMock.Object), Times.Once);
        }

        [Test]
        public void ResolveUninitialized_WhenUnwrapAndLinkReturnsLinkedType_UsesLinkedTypeForResolution()
        {
            var originalRequest = new FixtureRequest(typeof(IList<int>));
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(IList<int>))).Returns(typeof(List<int>));
            _valueProviderMock.Setup(c => c.ResolveValue(It.Is<FixtureRequest>(r => r.Type == typeof(List<int>)), _contextMock.Object))
                .Returns(new List<int>());
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(originalRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.TypeOf<List<int>>());
                Assert.That(originalRequest.Type, Is.EqualTo(typeof(List<int>)));
            }
        }

        [Test]
        public void ResolveUninitialized_WhenUnwrapAndLinkReturnsOriginalType_UsesOriginalType()
        {
            var originalRequest = new FixtureRequest(typeof(string));
            _contextMock.Setup(c => c.UnwrapAndLink(typeof(string))).Returns(typeof(string));
            _valueProviderMock.Setup(c => c.ResolveValue(It.Is<FixtureRequest>(r => r.Type == typeof(string)), _contextMock.Object))
                .Returns("resolved");
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            var result = sut.ResolveUninitialized(originalRequest, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.EqualTo("resolved"));
                Assert.That(originalRequest.Type, Is.EqualTo(typeof(string)));
            }
        }

        [Test]
        public void ResolveUninitialized_UnwrapAndLinkIsCalledBeforeResolveValue()
        {
            var callOrder = new List<string>();
            _contextMock.Setup(c => c.UnwrapAndLink(It.IsAny<Type>()))
                .Callback(() => callOrder.Add("UnwrapAndLink"))
                .Returns(typeof(string));
            _valueProviderMock.Setup(c => c.ResolveValue(It.IsAny<FixtureRequest>(), _contextMock.Object))
                .Callback(() => callOrder.Add("ResolveValue"))
                .Returns("result");
            var sut = new CompositeUninitializedProvider(_defaultBclTypeProvider);

            sut.ResolveUninitialized(_request, DefaultInitializeMembers, _contextMock.Object, _recursiveResolveContext);

            Assert.That(callOrder, Is.EqualTo(["UnwrapAndLink", "ResolveValue"]));
        }
    }
}
