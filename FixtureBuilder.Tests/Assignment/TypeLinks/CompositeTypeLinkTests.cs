using FixtureBuilder.Assignment.TypeLinks;
using Moq;

namespace FixtureBuilder.Tests.Assignment.TypeLinks
{
    internal sealed class CompositeTypeLinkTests
    {
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
            var targetType = typeof(string);

            var result = composite.Link(targetType);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Link_NoConverterReturnsResult_ReturnsNull()
        {
            var typeLink1 = new Mock<ITypeLink>();
            typeLink1.Setup(x => x.Link(It.IsAny<Type>())).Returns((Type?)null);

            var typeLink2 = new Mock<ITypeLink>();
            typeLink2.Setup(x => x.Link(It.IsAny<Type>())).Returns((Type?)null);

            var composite = new CompositeTypeLink([typeLink1.Object, typeLink2.Object]);
            var targetType = typeof(string);

            var result = composite.Link(targetType);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Link_FirstConverterReturnsResult_ReturnsResult()
        {
            var expectedResult = typeof(int);
            var targetType = typeof(string);

            var typeLink1 = new Mock<ITypeLink>();
            typeLink1.Setup(x => x.Link(targetType)).Returns(expectedResult);

            var typeLink2 = new Mock<ITypeLink>();

            var composite = new CompositeTypeLink([typeLink1.Object, typeLink2.Object]);

            var result = composite.Link(targetType);

            Assert.That(result, Is.EqualTo(expectedResult));
            typeLink1.Verify(x => x.Link(targetType), Times.Once);
            typeLink2.Verify(x => x.Link(It.IsAny<Type>()), Times.Never);
        }

        [Test]
        public void Link_SecondConverterReturnsResult_ReturnsResult()
        {
            var expectedResult = typeof(int);
            var targetType = typeof(string);

            var typeLink1 = new Mock<ITypeLink>();
            typeLink1.Setup(x => x.Link(targetType)).Returns((Type?)null);

            var typeLink2 = new Mock<ITypeLink>();
            typeLink2.Setup(x => x.Link(targetType)).Returns(expectedResult);

            var composite = new CompositeTypeLink([typeLink1.Object, typeLink2.Object]);

            var result = composite.Link(targetType);

            Assert.That(result, Is.EqualTo(expectedResult));
            typeLink1.Verify(x => x.Link(targetType), Times.Once);
            typeLink2.Verify(x => x.Link(targetType), Times.Once);
        }

        [Test]
        public void Link_StopsAtFirstNonNullResult()
        {
            var firstResult = typeof(int);
            var targetType = typeof(string);

            var typeLink1 = new Mock<ITypeLink>();
            typeLink1.Setup(x => x.Link(targetType)).Returns((Type?)null);

            var typeLink2 = new Mock<ITypeLink>();
            typeLink2.Setup(x => x.Link(targetType)).Returns(firstResult);

            var typeLink3 = new Mock<ITypeLink>();
            typeLink3.Setup(x => x.Link(targetType)).Returns(typeof(long));

            var composite = new CompositeTypeLink([typeLink1.Object, typeLink2.Object, typeLink3.Object]);

            var result = composite.Link(targetType);

            Assert.That(result, Is.EqualTo(firstResult));
            typeLink3.Verify(x => x.Link(It.IsAny<Type>()), Times.Never);
        }
    }
}
