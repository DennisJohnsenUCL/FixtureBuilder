using FixtureBuilder.ValueConverters;
using FixtureBuilder.ValueConverters.Decorators;
using Moq;
using System.Collections;

namespace FixtureBuilder.Tests.ValueConverters.Decorators
{
    internal sealed class EnumerableElementCastingConverterTests
    {
        [Test]
        public void Constructor_InnerNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new EnumerableElementCastingConverter(null!));
        }

        [Test]
        public void Constructor_Constructs()
        {
            var inner = Mock.Of<IValueConverter>();

            Assert.DoesNotThrow(() => new EnumerableElementCastingConverter(inner));
        }

        [Test]
        public void Convert_NonEnumerableValue_PassesThroughToInner()
        {
            var targetType = typeof(List<string>);
            var value = 5;
            var expectedResult = "converted";

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value)).Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value), Times.Once);
        }

        [Test]
        public void Convert_NonEnumerableTargetType_PassesThroughToInner()
        {
            var targetType = typeof(long);
            var value = new List<int> { 1, 2, 3 };
            var expectedResult = "converted";

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value)).Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value), Times.Once);
        }

        [Test]
        public void Convert_StringTargetType_PassesThroughToInner()
        {
            var targetType = typeof(string);
            var value = new List<int> { 1, 2, 3 };
            var expectedResult = "converted";

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value)).Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value), Times.Once);
        }

        [Test]
        public void Convert_ArrayTargetType_PassesThroughToInner()
        {
            var targetType = typeof(int[]);
            var value = new List<int> { 1, 2, 3 };
            var expectedResult = new int[] { 1, 2, 3 };

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value)).Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value), Times.Once);
        }

        [Test]
        public void Convert_DictionaryTargetType_PassesThroughToInner()
        {
            var targetType = typeof(Dictionary<,>);
            var value = new List<int> { 1, 2, 3 };
            var expectedResult = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value)).Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value), Times.Once);
        }

        [Test]
        public void Convert_SameElementTypes_PassesThroughToInner()
        {
            var targetType = typeof(Stack<int>);
            var value = new List<int> { 1, 2, 3 };
            var expectedResult = "converted";

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value)).Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value), Times.Once);
        }

        [Test]
        public void Convert_DifferentElementTypes_CastsElementsBeforePassingToInner()
        {
            var targetType = typeof(List<object>);
            var value = new List<int> { 1, 2, 3 };
            var expectedResult = new List<object> { 1, 2, 3 };

            IEnumerable? capturedEnumerable = null;
            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, It.IsAny<IEnumerable>()))
                .Callback<Type, object>((_, v) => capturedEnumerable = v as IEnumerable)
                .Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.EqualTo(expectedResult));
                Assert.That(capturedEnumerable, Is.Not.Null);
                Assert.That(capturedEnumerable, Is.Not.SameAs(value));
                Assert.That(capturedEnumerable!.GetType().GetGenericArguments()[0], Is.EqualTo(typeof(object)));
            }
        }

        [Test]
        public void Convert_NonGenericEnumerableSource_CastsToTargetElementType()
        {
            var targetType = typeof(List<string>);
            var value = new ArrayList { "a", "b", "c" };
            var expectedResult = new List<string> { "a", "b", "c" };

            IEnumerable? capturedEnumerable = null;
            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, It.IsAny<IEnumerable>()))
                .Callback<Type, object>((t, v) => capturedEnumerable = v as IEnumerable)
                .Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.EqualTo(expectedResult));
                Assert.That(capturedEnumerable, Is.Not.Null);
                Assert.That(capturedEnumerable, Is.Not.SameAs(value));
                Assert.That(capturedEnumerable!.GetType().GetGenericArguments()[0], Is.EqualTo(typeof(string)));
            }
        }

        [Test]
        public void Convert_CannotCastElements_ThrowsException()
        {
            var targetType = typeof(List<long>);
            var value = new List<int> { 1, 2, 3 };

            var innerMock = new Mock<IValueConverter>();

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            Assert.Throws<InvalidCastException>(() => converter.Convert(targetType, value));
        }
    }
}
