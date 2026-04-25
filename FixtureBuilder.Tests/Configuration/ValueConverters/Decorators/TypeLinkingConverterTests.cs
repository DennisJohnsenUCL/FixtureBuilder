using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.Decorators;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.Decorators
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
            var request = new FixtureRequest(targetType);
            var value = "test";
            var contextMock = new Mock<IFixtureContext>();
            contextMock.Setup(c => c.UnwrapAndLink(targetType)).Returns(targetType);

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(request, value, contextMock.Object)).Returns((object?)null);

            var converter = new TypeLinkingConverter(innerMock.Object);

            var result = converter.Convert(request, value, contextMock.Object);

            Assert.That(result, Is.Null);
            innerMock.Verify(x => x.Convert(request, value, contextMock.Object), Times.Once);
        }

        [Test]
        public void Convert_ReturnsResult()
        {
            var targetType = typeof(string);
            var request = new FixtureRequest(targetType);
            var value = "test";
            var expectedResult = "converted";
            var contextMock = new Mock<IFixtureContext>();
            contextMock.Setup(c => c.UnwrapAndLink(targetType)).Returns(targetType);

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(request, value, contextMock.Object)).Returns(expectedResult);

            var converter = new TypeLinkingConverter(innerMock.Object);

            var result = converter.Convert(request, value, contextMock.Object);

            Assert.That(expectedResult, Is.EqualTo(result));
            innerMock.Verify(x => x.Convert(request, value, contextMock.Object), Times.Once);
        }

        [Test]
        public void Convert_UnwrapAndLinkReturnsLinkedType_UpdatesTarget()
        {
            var originalType = typeof(string);
            var request = new FixtureRequest(originalType);
            var linkedType = typeof(int);
            var value = "test";
            var contextMock = new Mock<IFixtureContext>();
            contextMock.Setup(c => c.UnwrapAndLink(originalType)).Returns(linkedType);

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(It.Is<FixtureRequest>(r => r.Type == linkedType), value, contextMock.Object)).Returns(42);

            var converter = new TypeLinkingConverter(innerMock.Object);

            converter.Convert(request, value, contextMock.Object);

            innerMock.Verify(x => x.Convert(It.Is<FixtureRequest>(r => r.Type == linkedType), value, contextMock.Object), Times.Once);
            innerMock.Verify(x => x.Convert(It.Is<FixtureRequest>(r => r.Type == originalType), It.IsAny<object>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void Convert_UnwrapAndLinkReturnsLinkedType_ReturnsResult()
        {
            var originalType = typeof(string);
            var request = new FixtureRequest(originalType);
            var linkedType = typeof(int);
            var value = "test";
            var expectedResult = 42;
            var contextMock = new Mock<IFixtureContext>();
            contextMock.Setup(c => c.UnwrapAndLink(originalType)).Returns(linkedType);

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(It.Is<FixtureRequest>(r => r.Type == linkedType), value, contextMock.Object)).Returns(expectedResult);

            var converter = new TypeLinkingConverter(innerMock.Object);

            var result = converter.Convert(request, value, contextMock.Object);

            Assert.That(expectedResult, Is.EqualTo(result));
        }
    }
}
