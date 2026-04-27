using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories.RootConfigurationBuilders;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.RootProviderBuilders
{
    internal sealed class RootProviderDecoratorTests
    {
        private Mock<IValueProvider> _innerMock;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _innerMock = new Mock<IValueProvider>();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void Constructor_NullProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => new RootProviderDecorator(null!, typeof(string)));
        }

        [Test]
        public void Constructor_NullRootType_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () => new RootProviderDecorator(_innerMock.Object, null!));
        }

        [Test]
        public void ResolveValue_MatchingRootType_DelegatesToInner()
        {
            var request = new FixtureRequest(typeof(string), "source", typeof(string), null);
            _innerMock
                .Setup(p => p.ResolveValue(request, _contextMock.Object))
                .Returns("resolved");
            var sut = new RootProviderDecorator(_innerMock.Object, typeof(string));

            var result = sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.EqualTo("resolved"));
            _innerMock.Verify(p => p.ResolveValue(request, _contextMock.Object), Times.Once);
        }

        [Test]
        public void ResolveValue_NonMatchingRootType_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(string), "source", typeof(string), null);
            var sut = new RootProviderDecorator(_innerMock.Object, typeof(int));

            var result = sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
            _innerMock.Verify(
                p => p.ResolveValue(It.IsAny<FixtureRequest>(), It.IsAny<IFixtureContext>()),
                Times.Never);
        }

        [Test]
        public void ResolveValue_InnerReturnsNull_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(string), "source", typeof(string), null);
            _innerMock
                .Setup(p => p.ResolveValue(request, _contextMock.Object))
                .Returns((object?)null);
            var sut = new RootProviderDecorator(_innerMock.Object, typeof(string));

            var result = sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void ResolveValue_PassesContextToInner()
        {
            var request = new FixtureRequest(typeof(string), "source", typeof(string), null);
            var sut = new RootProviderDecorator(_innerMock.Object, typeof(string));

            sut.ResolveValue(request, _contextMock.Object);

            _innerMock.Verify(p => p.ResolveValue(request, _contextMock.Object), Times.Once);
        }
    }
}
