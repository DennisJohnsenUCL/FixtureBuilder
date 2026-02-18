using FixtureBuilder.FixtureContexts;
using FixtureBuilder.ValueConverters;
using FixtureBuilder.ValueConverters.Decorators;
using Moq;

namespace FixtureBuilder.Tests.ValueConverters.Decorators
{
    internal sealed class CompositeConverterTests
    {
        [Test]
        public void Constructor_ConvertersNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new CompositeConverter(null!));
        }

        [Test]
        public void Constructor_Constructs()
        {
            var converters = new[] { Mock.Of<IValueConverter>(), Mock.Of<IValueConverter>() };

            Assert.DoesNotThrow(() => new CompositeConverter(converters));
        }

        [Test]
        public void Convert_NoConverters_ReturnsNull()
        {
            var converters = Enumerable.Empty<IValueConverter>();
            var composite = new CompositeConverter(converters);
            var targetType = typeof(string);
            var value = "test";
            var context = new Mock<IFixtureContext>().Object;

            var result = composite.Convert(targetType, value, context);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_NoConverterReturnsResult_ReturnsNull()
        {
            var converter1 = new Mock<IValueConverter>();
            converter1.Setup(x => x.Convert(It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<IFixtureContext>())).Returns((object?)null);

            var converter2 = new Mock<IValueConverter>();
            converter2.Setup(x => x.Convert(It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<IFixtureContext>())).Returns((object?)null);

            var composite = new CompositeConverter([converter1.Object, converter2.Object]);
            var targetType = typeof(string);
            var value = "test";
            var context = new Mock<IFixtureContext>().Object;

            var result = composite.Convert(targetType, value, context);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_FirstConverterReturnsResult_ReturnsResult()
        {
            var expectedResult = "converted";
            var targetType = typeof(string);
            var value = "test";
            var context = new Mock<IFixtureContext>().Object;

            var converter1 = new Mock<IValueConverter>();
            converter1.Setup(x => x.Convert(targetType, value, context)).Returns(expectedResult);

            var converter2 = new Mock<IValueConverter>();

            var composite = new CompositeConverter([converter1.Object, converter2.Object]);

            var result = composite.Convert(targetType, value, context);

            Assert.That(result, Is.EqualTo(expectedResult));
            converter1.Verify(x => x.Convert(targetType, value, context), Times.Once);
            converter2.Verify(x => x.Convert(It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void Convert_SecondConverterReturnsResult_ReturnsResult()
        {
            var expectedResult = "converted";
            var targetType = typeof(string);
            var value = "test";
            var context = new Mock<IFixtureContext>().Object;

            var converter1 = new Mock<IValueConverter>();
            converter1.Setup(x => x.Convert(targetType, value, context)).Returns((object?)null);

            var converter2 = new Mock<IValueConverter>();
            converter2.Setup(x => x.Convert(targetType, value, context)).Returns(expectedResult);

            var composite = new CompositeConverter([converter1.Object, converter2.Object]);

            var result = composite.Convert(targetType, value, context);

            Assert.That(result, Is.EqualTo(expectedResult));
            converter1.Verify(x => x.Convert(targetType, value, context), Times.Once);
            converter2.Verify(x => x.Convert(targetType, value, context), Times.Once);
        }

        [Test]
        public void Convert_StopsAtFirstNonNullResult()
        {
            var firstResult = "first";
            var targetType = typeof(string);
            var value = "test";
            var context = new Mock<IFixtureContext>().Object;

            var converter1 = new Mock<IValueConverter>();
            converter1.Setup(x => x.Convert(targetType, value, context)).Returns((object?)null);

            var converter2 = new Mock<IValueConverter>();
            converter2.Setup(x => x.Convert(targetType, value, context)).Returns(firstResult);

            var converter3 = new Mock<IValueConverter>();
            converter3.Setup(x => x.Convert(targetType, value, context)).Returns("should not reach");

            var composite = new CompositeConverter([converter1.Object, converter2.Object, converter3.Object]);

            var result = composite.Convert(targetType, value, context);

            Assert.That(result, Is.EqualTo(firstResult));
            converter3.Verify(x => x.Convert(It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()), Times.Never);
        }
    }
}
