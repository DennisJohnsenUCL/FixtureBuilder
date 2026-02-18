using FixtureBuilder.FixtureContexts;
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
            Assert.Throws<ArgumentNullException>(() => new TypeLinkingConverter(null!));
        }

        [Test]
        public void Constructor_Constructs()
        {
            var inner = Mock.Of<IValueConverter>();

            Assert.DoesNotThrow(() => new TypeLinkingConverter(inner));
        }

        [Test]
        public void Convert_NoResult_ReturnsNull()
        {
            var targetType = typeof(string);
            var value = "test";
            var context = new Mock<IFixtureContext>().Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value, context)).Returns((object?)null);

            var converter = new TypeLinkingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

            Assert.That(result, Is.Null);
            innerMock.Verify(x => x.Convert(targetType, value, context), Times.Once);
        }

        [Test]
        public void Convert_ReturnsResult()
        {
            var targetType = typeof(string);
            var value = "test";
            var expectedResult = "converted";
            var context = new Mock<IFixtureContext>().Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value, context)).Returns(expectedResult);

            var typeLinkMock = new Mock<ITypeLink>();
            typeLinkMock.Setup(x => x.Link(targetType)).Returns((Type?)null);

            var converter = new TypeLinkingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

            Assert.That(expectedResult, Is.EqualTo(result));
            innerMock.Verify(x => x.Convert(targetType, value, context), Times.Once);
        }

        [Test]
        public void Convert_TypeLink_UpdatesTarget()
        {
            var originalType = typeof(string);
            var linkedType = typeof(int);
            var value = "test";
            var contextMock = new Mock<IFixtureContext>();
            contextMock.Setup(x => x.Link(originalType)).Returns(linkedType);
            var context = contextMock.Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(linkedType, value, context)).Returns(42);

            var converter = new TypeLinkingConverter(innerMock.Object);

            converter.Convert(originalType, value, context);

            innerMock.Verify(x => x.Convert(linkedType, value, context), Times.Once);
            innerMock.Verify(x => x.Convert(originalType, It.IsAny<object>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void Convert_TypeLink_ReturnsResult()
        {
            var originalType = typeof(string);
            var linkedType = typeof(int);
            var value = "test";
            var expectedResult = 42;
            var contextMock = new Mock<IFixtureContext>();
            contextMock.Setup(x => x.Link(originalType)).Returns(linkedType);
            var context = contextMock.Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(linkedType, value, context)).Returns(expectedResult);

            var converter = new TypeLinkingConverter(innerMock.Object);

            var result = converter.Convert(originalType, value, context);

            Assert.That(expectedResult, Is.EqualTo(result));
        }
    }
}
