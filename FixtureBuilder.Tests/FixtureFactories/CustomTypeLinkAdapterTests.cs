using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories
{
    internal sealed class CustomTypeLinkAdapterTests
    {
        private Mock<ICustomTypeLink> _adapteeMock;

        [SetUp]
        public void SetUp()
        {
            _adapteeMock = new Mock<ICustomTypeLink>();
        }

        [Test]
        public void Constructor_NullAdaptee_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomTypeLinkAdapter(null!));
        }

        [Test]
        public void Link_DelegatesToAdapteeWithRequestType()
        {
            var request = new FixtureRequest(typeof(string));
            _adapteeMock.Setup(a => a.Link(typeof(string))).Returns(typeof(int));
            var sut = new CustomTypeLinkAdapter(_adapteeMock.Object);

            var result = sut.Link(request);

            Assert.That(result, Is.EqualTo(typeof(int)));
            _adapteeMock.Verify(a => a.Link(typeof(string)), Times.Once);
        }

        [Test]
        public void Link_AdapteeReturnsNull_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(string));
            _adapteeMock.Setup(a => a.Link(typeof(string))).Returns((Type?)null);
            var sut = new CustomTypeLinkAdapter(_adapteeMock.Object);

            var result = sut.Link(request);

            Assert.That(result, Is.Null);
        }
    }
}
