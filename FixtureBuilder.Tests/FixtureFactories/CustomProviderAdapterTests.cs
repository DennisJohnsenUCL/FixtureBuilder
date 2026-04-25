using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories
{
    internal sealed class CustomProviderAdapterTests
    {
        private Mock<ICustomProvider> _adapteeMock;
        private Mock<IFixtureContext> _contextMock;
        private FixtureRequest _request;

        [SetUp]
        public void SetUp()
        {
            _adapteeMock = new Mock<ICustomProvider>();
            _contextMock = new Mock<IFixtureContext>();
            _request = new FixtureRequest(typeof(string));
        }

        [Test]
        public void Constructor_NullAdaptee_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomProviderAdapter(null!));
        }

        [Test]
        public void ResolveValue_DelegatesToAdapteeWithRequest()
        {
            _adapteeMock.Setup(a => a.ResolveValue(_request)).Returns("resolved");
            var sut = new CustomProviderAdapter(_adapteeMock.Object);

            var result = sut.ResolveValue(_request, _contextMock.Object);

            Assert.That(result, Is.EqualTo("resolved"));
            _adapteeMock.Verify(a => a.ResolveValue(_request), Times.Once);
        }

        [Test]
        public void ResolveValue_DoesNotPassContextToAdaptee()
        {
            _adapteeMock.Setup(a => a.ResolveValue(It.IsAny<FixtureRequest>())).Returns("resolved");
            var sut = new CustomProviderAdapter(_adapteeMock.Object);

            sut.ResolveValue(_request, _contextMock.Object);

            _contextMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Constructor_WithRootType_NullAdaptee_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomProviderAdapter(null!, typeof(string)));
        }

        [Test]
        public void Constructor_WithRootType_NullRootType_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomProviderAdapter(_adapteeMock.Object, null!));
        }

        [Test]
        public void ResolveValue_WithMatchingRootType_DelegatesToAdaptee()
        {
            _adapteeMock.Setup(a => a.ResolveValue(It.IsAny<FixtureRequest>())).Returns("resolved");
            var sut = new CustomProviderAdapter(_adapteeMock.Object, typeof(string));
            var request = new FixtureRequest(typeof(string), "source", typeof(string), null);

            var result = sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.EqualTo("resolved"));
        }

        [Test]
        public void ResolveValue_WithNonMatchingRootType_ReturnsNoResult()
        {
            var sut = new CustomProviderAdapter(_adapteeMock.Object, typeof(int));
            var request = new FixtureRequest(typeof(string), "source", typeof(string), null);

            var result = sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
            _adapteeMock.Verify(a => a.ResolveValue(It.IsAny<FixtureRequest>()), Times.Never);
        }

        [Test]
        public void ResolveValue_WithoutRootType_AlwaysDelegates()
        {
            _adapteeMock.Setup(a => a.ResolveValue(It.IsAny<FixtureRequest>())).Returns("resolved");
            var sut = new CustomProviderAdapter(_adapteeMock.Object);
            var request = new FixtureRequest(typeof(string), "source", typeof(int), null);

            var result = sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.EqualTo("resolved"));
        }
    }
}
