using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;
using Moq;

namespace FixtureBuilder.Tests.Assignment.TypeLinks
{
    internal sealed class CompositeTypeLinkTests
    {
        private FixtureRequest _request;

        [SetUp]
        public void SetUp()
        {
            _request = new FixtureRequest(typeof(string));
        }

        [Test]
        public void Constructor_TypeLinksNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new CompositeTypeLink(null!));
        }

        [Test]
        public void Constructor_Constructs()
        {
            var typeLinks = new[] { Mock.Of<ITypeLink>(), Mock.Of<ITypeLink>() };

            Assert.DoesNotThrow(() => new CompositeTypeLink(typeLinks));
        }

        [Test]
        public void Link_Notypelinks_ReturnsNull()
        {
            var typelinks = Enumerable.Empty<ITypeLink>();
            var composite = new CompositeTypeLink(typelinks);

            var result = composite.Link(_request);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Link_NoConverterReturnsResult_ReturnsNull()
        {
            var typeLink1 = new Mock<ITypeLink>();
            typeLink1.Setup(x => x.Link(It.IsAny<FixtureRequest>())).Returns((Type?)null);

            var typeLink2 = new Mock<ITypeLink>();
            typeLink2.Setup(x => x.Link(It.IsAny<FixtureRequest>())).Returns((Type?)null);

            var composite = new CompositeTypeLink([typeLink1.Object, typeLink2.Object]);

            var result = composite.Link(_request);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Link_FirstConverterReturnsResult_ReturnsResult()
        {
            var expectedResult = typeof(int);

            var typeLink1 = new Mock<ITypeLink>();
            typeLink1.Setup(x => x.Link(_request)).Returns(expectedResult);

            var typeLink2 = new Mock<ITypeLink>();

            var composite = new CompositeTypeLink([typeLink1.Object, typeLink2.Object]);

            var result = composite.Link(_request);

            Assert.That(result, Is.EqualTo(expectedResult));
            typeLink1.Verify(x => x.Link(_request), Times.Once);
            typeLink2.Verify(x => x.Link(It.IsAny<FixtureRequest>()), Times.Never);
        }

        [Test]
        public void Link_SecondConverterReturnsResult_ReturnsResult()
        {
            var expectedResult = typeof(int);

            var typeLink1 = new Mock<ITypeLink>();
            typeLink1.Setup(x => x.Link(_request)).Returns((Type?)null);

            var typeLink2 = new Mock<ITypeLink>();
            typeLink2.Setup(x => x.Link(_request)).Returns(expectedResult);

            var composite = new CompositeTypeLink([typeLink1.Object, typeLink2.Object]);

            var result = composite.Link(_request);

            Assert.That(result, Is.EqualTo(expectedResult));
            typeLink1.Verify(x => x.Link(_request), Times.Once);
            typeLink2.Verify(x => x.Link(_request), Times.Once);
        }

        [Test]
        public void Link_StopsAtFirstNonNullResult()
        {
            var firstResult = typeof(int);

            var typeLink1 = new Mock<ITypeLink>();
            typeLink1.Setup(x => x.Link(_request)).Returns((Type?)null);

            var typeLink2 = new Mock<ITypeLink>();
            typeLink2.Setup(x => x.Link(_request)).Returns(firstResult);

            var typeLink3 = new Mock<ITypeLink>();
            typeLink3.Setup(x => x.Link(_request)).Returns(typeof(long));

            var composite = new CompositeTypeLink([typeLink1.Object, typeLink2.Object, typeLink3.Object]);

            var result = composite.Link(_request);

            Assert.That(result, Is.EqualTo(firstResult));
            typeLink3.Verify(x => x.Link(It.IsAny<FixtureRequest>()), Times.Never);
        }

        [Test]
        public void AddTypeLink_AddedLinkIsUsedByLink()
        {
            var expectedResult = typeof(int);

            var addedLink = new Mock<ITypeLink>();
            addedLink.Setup(x => x.Link(_request)).Returns(expectedResult);

            var composite = new CompositeTypeLink([]);
            composite.AddTypeLink(addedLink.Object);

            var result = composite.Link(_request);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void AddTypeLink_PrependedBeforeExistingLinks()
        {
            var addedResult = typeof(int);
            var originalResult = typeof(long);

            var originalLink = new Mock<ITypeLink>();
            originalLink.Setup(x => x.Link(_request)).Returns(originalResult);

            var addedLink = new Mock<ITypeLink>();
            addedLink.Setup(x => x.Link(_request)).Returns(addedResult);

            var composite = new CompositeTypeLink([originalLink.Object]);
            composite.AddTypeLink(addedLink.Object);

            var result = composite.Link(_request);

            Assert.That(result, Is.EqualTo(addedResult));
            originalLink.Verify(x => x.Link(It.IsAny<FixtureRequest>()), Times.Never);
        }
    }
}
