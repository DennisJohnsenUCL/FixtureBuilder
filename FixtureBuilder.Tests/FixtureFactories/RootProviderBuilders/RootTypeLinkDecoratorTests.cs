using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories.WithMatching;
using Moq;

namespace FixtureBuilder.Tests.FixtureFactories.RootProviderBuilders
{
    internal sealed class RootTypeLinkDecoratorTests
    {
        private Mock<ITypeLink> _innerMock;

        [SetUp]
        public void SetUp()
        {
            _innerMock = new Mock<ITypeLink>();
        }

        [Test]
        public void Constructor_NullInner_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new RootTypeLinkDecorator(null!, typeof(string)));
        }

        [Test]
        public void Constructor_NullRootType_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new RootTypeLinkDecorator(_innerMock.Object, null!));
        }

        [Test]
        public void Link_MatchingRootType_DelegatesToInner()
        {
            _innerMock.Setup(i => i.Link(It.IsAny<FixtureRequest>())).Returns(typeof(int));
            var sut = new RootTypeLinkDecorator(_innerMock.Object, typeof(string));
            var request = new FixtureRequest(typeof(double), "source", typeof(string), null);

            var result = sut.Link(request);

            Assert.That(result, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void Link_NonMatchingRootType_ReturnsNull()
        {
            var sut = new RootTypeLinkDecorator(_innerMock.Object, typeof(string));
            var request = new FixtureRequest(typeof(double), "source", typeof(int), null);

            var result = sut.Link(request);

            Assert.That(result, Is.Null);
            _innerMock.Verify(i => i.Link(It.IsAny<FixtureRequest>()), Times.Never);
        }
    }
}
