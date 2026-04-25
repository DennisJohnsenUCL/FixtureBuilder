using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.Decorators;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.Decorators
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
            var context = new Mock<IFixtureContext>().Object;
            var value = "Test string";

            var cut = new ValidatingConverter(inner);

            Assert.Throws<ArgumentNullException>(() => cut.Convert(null!, value, context));
        }

        [Test]
        public void Convert_ValueNull_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(string));
            var inner = new Mock<IValueConverter>().Object;
            var context = new Mock<IFixtureContext>().Object;

            var cut = new ValidatingConverter(inner);

            var actual = cut.Convert(request, null!, context);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Convert_ContextNull_ThrowsException()
        {
            var inner = new Mock<IValueConverter>().Object;
            var target = new FixtureRequest(typeof(int));
            var value = "Test string";

            var cut = new ValidatingConverter(inner);

            Assert.Throws<ArgumentNullException>(() => cut.Convert(target, value, null!));
        }

        [Test]
        public void Convert_TargetEqualsValueType_ReturnsValue()
        {
            var request = new FixtureRequest(typeof(string));
            var inner = new Mock<IValueConverter>().Object;
            var context = new Mock<IFixtureContext>().Object;

            var cut = new ValidatingConverter(inner);

            var expected = "Test string";
            var actual = cut.Convert(request, expected, context);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_InnerReturnsNull_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(int));
            var context = new Mock<IFixtureContext>().Object;
            var mock = new Mock<IValueConverter>();
            mock.Setup(m => m.Convert(It.IsAny<FixtureRequest>(), It.IsAny<object>(), context))
                .Returns(null!);
            var inner = mock.Object;

            var cut = new ValidatingConverter(inner);

            var result = cut.Convert(request, "Any string", context);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_InnerReturnsWrongType_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(int));
            var expected = "Test string";
            var context = new Mock<IFixtureContext>().Object;

            var mock = new Mock<IValueConverter>();
            mock.Setup(m => m.Convert(It.IsAny<FixtureRequest>(), It.IsAny<object>(), context))
                .Returns(expected);
            var inner = mock.Object;

            var cut = new ValidatingConverter(inner);

            var result = cut.Convert(request, "Any string", context);
            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_InnerReturnsValue_ReturnsValue()
        {
            var request = new FixtureRequest(typeof(string));
            var expected = "Test string";
            var context = new Mock<IFixtureContext>().Object;

            var mock = new Mock<IValueConverter>();
            mock.Setup(m => m.Convert(It.IsAny<FixtureRequest>(), It.IsAny<object>(), context))
                .Returns(expected);
            var inner = mock.Object;

            var cut = new ValidatingConverter(inner);

            var actual = cut.Convert(request, 5, context);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
