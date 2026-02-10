using FixtureBuilder.TypeLinks;
using FixtureBuilder.ValueConverters;
using FixtureBuilder.ValueConverters.Decorators;
using Moq;

namespace FixtureBuilder.Tests.ValueConverters.Decorators
{
    internal sealed class TypeLinkingConverterTests
    {
        [Test]
        public void Constructor_InnerNull_ThrowsException()
        {
            var typeLink = Mock.Of<ITypeLink>();

            Assert.Throws<ArgumentNullException>(() => new TypeLinkingConverter(null!, typeLink));
        }

        [Test]
        public void Constructor_TypeLinkNull_ThrowsException()
        {
            var inner = Mock.Of<IValueConverter>();

            Assert.Throws<ArgumentNullException>(() => new TypeLinkingConverter(inner, null!));
        }

        [Test]
        public void Constructor_Constructs()
        {
            var inner = Mock.Of<IValueConverter>();
            var typeLink = Mock.Of<ITypeLink>();

            Assert.DoesNotThrow(() => new TypeLinkingConverter(inner, typeLink));
        }

        [Test]
        public void Convert_NoResult_ReturnsNull()
        {
            var targetType = typeof(string);
            var value = "test";

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value)).Returns((object?)null);

            var typeLinkMock = new Mock<ITypeLink>();
            typeLinkMock.Setup(x => x.Link(targetType)).Returns((Type?)null);

            var converter = new TypeLinkingConverter(innerMock.Object, typeLinkMock.Object);

            var result = converter.Convert(targetType, value);

            Assert.That(result, Is.Null);
            innerMock.Verify(x => x.Convert(targetType, value), Times.Once);
        }

        [Test]
        public void Convert_ReturnsResult()
        {
            var targetType = typeof(string);
            var value = "test";
            var expectedResult = "converted";

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value)).Returns(expectedResult);

            var typeLinkMock = new Mock<ITypeLink>();
            typeLinkMock.Setup(x => x.Link(targetType)).Returns((Type?)null);

            var converter = new TypeLinkingConverter(innerMock.Object, typeLinkMock.Object);

            var result = converter.Convert(targetType, value);

            Assert.That(expectedResult, Is.EqualTo(result));
            innerMock.Verify(x => x.Convert(targetType, value), Times.Once);
        }

        [Test]
        public void Convert_TypeLink_UpdatesTarget()
        {
            var originalType = typeof(string);
            var linkedType = typeof(int);
            var value = "test";

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(linkedType, value)).Returns(42);

            var typeLinkMock = new Mock<ITypeLink>();
            typeLinkMock.Setup(x => x.Link(originalType)).Returns(linkedType);

            var converter = new TypeLinkingConverter(innerMock.Object, typeLinkMock.Object);

            converter.Convert(originalType, value);

            typeLinkMock.Verify(x => x.Link(originalType), Times.Once);
            innerMock.Verify(x => x.Convert(linkedType, value), Times.Once);
            innerMock.Verify(x => x.Convert(originalType, It.IsAny<object>()), Times.Never);
        }

        [Test]
        public void Convert_TypeLink_ReturnsResult()
        {
            var originalType = typeof(string);
            var linkedType = typeof(int);
            var value = "test";
            var expectedResult = 42;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(linkedType, value)).Returns(expectedResult);

            var typeLinkMock = new Mock<ITypeLink>();
            typeLinkMock.Setup(x => x.Link(originalType)).Returns(linkedType);

            var converter = new TypeLinkingConverter(innerMock.Object, typeLinkMock.Object);

            var result = converter.Convert(originalType, value);

            Assert.That(expectedResult, Is.EqualTo(result));
        }
    }
}
