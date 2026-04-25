using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.Decorators
{
    internal sealed class CompositeConverterTests
    {
        private IFixtureContext _context;

        [SetUp]
        public void Setup()
        {
            _context = new Mock<IFixtureContext>().Object;
        }

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
        public void Convert_NoConverters_ReturnsNoResult()
        {
            var converters = Enumerable.Empty<IValueConverter>();
            var composite = new CompositeConverter(converters);
            var targetType = new FixtureRequest(typeof(string));
            var value = "test";

            var result = composite.Convert(targetType, value, _context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_NoConverterReturnsResult_ReturnsNull()
        {
            var converter1 = new Mock<IValueConverter>();
            converter1.Setup(x => x.Convert(It.IsAny<FixtureRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>())).Returns(new NoResult());

            var converter2 = new Mock<IValueConverter>();
            converter2.Setup(x => x.Convert(It.IsAny<FixtureRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>())).Returns(new NoResult());

            var composite = new CompositeConverter([converter1.Object, converter2.Object]);
            var targetType = new FixtureRequest(typeof(string));
            var value = "test";

            var result = composite.Convert(targetType, value, _context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_FirstConverterReturnsResult_ReturnsResult()
        {
            var expectedResult = "converted";
            var targetType = new FixtureRequest(typeof(string));
            var value = "test";

            var converter1 = new Mock<IValueConverter>();
            converter1.Setup(x => x.Convert(targetType, value, _context)).Returns(expectedResult);

            var converter2 = new Mock<IValueConverter>();

            var composite = new CompositeConverter([converter1.Object, converter2.Object]);

            var result = composite.Convert(targetType, value, _context);

            Assert.That(result, Is.EqualTo(expectedResult));
            converter1.Verify(x => x.Convert(targetType, value, _context), Times.Once);
            converter2.Verify(x => x.Convert(It.IsAny<FixtureRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void Convert_SecondConverterReturnsResult_ReturnsResult()
        {
            var expectedResult = "converted";
            var targetType = new FixtureRequest(typeof(string));
            var value = "test";

            var converter1 = new Mock<IValueConverter>();
            converter1.Setup(x => x.Convert(targetType, value, _context)).Returns(new NoResult());

            var converter2 = new Mock<IValueConverter>();
            converter2.Setup(x => x.Convert(targetType, value, _context)).Returns(expectedResult);

            var composite = new CompositeConverter([converter1.Object, converter2.Object]);

            var result = composite.Convert(targetType, value, _context);

            Assert.That(result, Is.EqualTo(expectedResult));
            converter1.Verify(x => x.Convert(targetType, value, _context), Times.Once);
            converter2.Verify(x => x.Convert(targetType, value, _context), Times.Once);
        }

        [Test]
        public void Convert_StopsAtFirstNonNoResultResult()
        {
            var firstResult = "first";
            var targetType = new FixtureRequest(typeof(string));
            var value = "test";

            var converter1 = new Mock<IValueConverter>();
            converter1.Setup(x => x.Convert(targetType, value, _context)).Returns(new NoResult());

            var converter2 = new Mock<IValueConverter>();
            converter2.Setup(x => x.Convert(targetType, value, _context)).Returns(firstResult);

            var converter3 = new Mock<IValueConverter>();
            converter3.Setup(x => x.Convert(targetType, value, _context)).Returns("should not reach");

            var composite = new CompositeConverter([converter1.Object, converter2.Object, converter3.Object]);

            var result = composite.Convert(targetType, value, _context);

            Assert.That(result, Is.EqualTo(firstResult));
            converter3.Verify(x => x.Convert(It.IsAny<FixtureRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void AddConverter_AddedConverterIsUsedByConvert()
        {
            var targetType = new FixtureRequest(typeof(string));
            var value = "test";
            var expected = "converted";

            var addedConverter = new Mock<IValueConverter>();
            addedConverter.Setup(x => x.Convert(targetType, value, _context)).Returns(expected);

            var sut = new CompositeConverter([]);
            sut.AddConverter(addedConverter.Object);

            var result = sut.Convert(targetType, value, _context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void AddConverter_PrependedBeforeExistingConverters()
        {
            var targetType = new FixtureRequest(typeof(string));
            var value = "test";
            var addedResult = "from-added";
            var originalResult = "from-original";

            var originalConverter = new Mock<IValueConverter>();
            originalConverter.Setup(x => x.Convert(targetType, value, _context)).Returns(originalResult);

            var addedConverter = new Mock<IValueConverter>();
            addedConverter.Setup(x => x.Convert(targetType, value, _context)).Returns(addedResult);

            var sut = new CompositeConverter([originalConverter.Object]);
            sut.AddConverter(addedConverter.Object);

            var result = sut.Convert(targetType, value, _context);

            Assert.That(result, Is.EqualTo(addedResult));
            originalConverter.Verify(x => x.Convert(It.IsAny<FixtureRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()), Times.Never);
        }
    }
}
