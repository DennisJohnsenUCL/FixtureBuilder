using FixtureBuilder.ValueConverters;
using FixtureBuilder.ValueConverters.Decorators;
using Moq;

namespace FixtureBuilder.Tests.ValueConverters.Decorators
{
    internal sealed class ValidatingConverterTests
    {
        [Test]
        public void Constructor_InnerNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new ValidatingConverter(null!));
        }

        [Test]
        public void Constructor_Constructs()
        {
            var inner = new Mock<IValueConverter>().Object;

            Assert.DoesNotThrow(() => new ValidatingConverter(inner));
        }

        [Test]
        public void Convert_TargetNull_ThrowsException()
        {
            var inner = new Mock<IValueConverter>().Object;

            var cut = new ValidatingConverter(inner);

            Assert.Throws<ArgumentNullException>(() => cut.Convert(null!, null!));
        }

        [Test]
        public void Convert_ValueNull_ReturnsNull()
        {
            var inner = new Mock<IValueConverter>().Object;

            var cut = new ValidatingConverter(inner);

            var actual = cut.Convert(typeof(string), null!);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Convert_TargetEqualsValueType_ReturnsValue()
        {
            var inner = new Mock<IValueConverter>().Object;

            var cut = new ValidatingConverter(inner);

            var expected = "Test string";
            var actual = cut.Convert(typeof(string), expected);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_InnerReturnsNull_ThrowsException()
        {
            var mock = new Mock<IValueConverter>();
            mock.Setup(m => m.Convert(It.IsAny<Type>(), It.IsAny<object>()))
                .Returns(null!);
            var inner = mock.Object;

            var cut = new ValidatingConverter(inner);

            Assert.Throws<InvalidOperationException>(() => cut.Convert(typeof(int), "Any string"));
        }

        [Test]
        public void Convert_InnerReturnsWrongType_ThrowsException()
        {
            var expected = "Test string";

            var mock = new Mock<IValueConverter>();
            mock.Setup(m => m.Convert(It.IsAny<Type>(), It.IsAny<object>()))
                .Returns(expected);
            var inner = mock.Object;

            var cut = new ValidatingConverter(inner);

            Assert.Throws<InvalidOperationException>(() => cut.Convert(typeof(int), "Any string"));
        }

        [Test]
        public void Convert_InnerReturnsValue_ReturnsValue()
        {
            var expected = "Test string";

            var mock = new Mock<IValueConverter>();
            mock.Setup(m => m.Convert(It.IsAny<Type>(), It.IsAny<object>()))
                .Returns(expected);
            var inner = mock.Object;

            var cut = new ValidatingConverter(inner);

            var actual = cut.Convert(typeof(string), 5);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
