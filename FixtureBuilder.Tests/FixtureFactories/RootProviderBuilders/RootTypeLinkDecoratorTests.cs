using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories.RootConfigurationBuilders;
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
            _innerMock.Setup(i => i.Link(It.IsAny<TypeRequest>())).Returns(typeof(int));
            var sut = new RootTypeLinkDecorator(_innerMock.Object, typeof(string));
            var request = new TypeRequest(typeof(double), typeof(string));

            var result = sut.Link(request);

            Assert.That(result, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void Link_NonMatchingRootType_ReturnsNull()
        {
            var sut = new RootTypeLinkDecorator(_innerMock.Object, typeof(string));
            var request = new TypeRequest(typeof(double), typeof(int));

            var result = sut.Link(request);

            Assert.That(result, Is.Null);
            _innerMock.Verify(i => i.Link(It.IsAny<TypeRequest>()), Times.Never);
        }
    }
}
