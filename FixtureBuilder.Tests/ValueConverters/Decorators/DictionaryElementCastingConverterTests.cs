using FixtureBuilder.ValueConverters;
using FixtureBuilder.ValueConverters.Decorators;
using Moq;
using System.Collections;

namespace FixtureBuilder.Tests.ValueConverters.Decorators
{
    internal sealed class DictionaryElementCastingConverterTests
    {
        [Test]
        public void Constructor_InnerNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new DictionaryElementCastingConverter(null!));
        }

        [Test]
        public void Constructor_Constructs()
        {
            var inner = Mock.Of<IValueConverter>();

            Assert.DoesNotThrow(() => new DictionaryElementCastingConverter(inner));
        }

        [Test]
        public void Convert_NonEnumerableValue_PassesThroughToInner()
        {
            var targetType = typeof(Dictionary<string, int>);
            var value = 5;
            var expectedResult = "converted";

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value)).Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value), Times.Once);
        }

        [Test]
        public void Convert_ValueNotDictionaryOrEnumerableKvp_PassesThroughToInner()
        {
            var targetType = typeof(Dictionary<string, int>);
            var value = new List<string> { "one", "two", "three" };
            var expectedResult = "converted";

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value)).Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value), Times.Once);
        }

        [Test]
        public void Convert_TargetTypeNotDictionary_PassesThroughToInner()
        {
            var targetType = typeof(List<string>);
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expectedResult = "converted";

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value)).Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value), Times.Once);
        }

        [Test]
        public void Convert_NonGenericDictionaryTargetType_PassesThroughToInner()
        {
            var targetType = typeof(Hashtable);
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expectedResult = "converted";

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value)).Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value), Times.Once);
        }

        [Test]
        public void Convert_SameElementTypes_PassesThroughToInner()
        {
            var targetType = typeof(Dictionary<string, int>);
            var value = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expectedResult = "converted";

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value)).Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value), Times.Once);
        }

        [Test]
        public void Convert_GenericDictionaryValue_DifferentElementTypes_CastsElements()
        {
            var targetType = typeof(Dictionary<object, object>);
            var value = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expectedResult = new Dictionary<object, object> { { "one", 1 }, { "two", 2 }, { "three", 3 } };

            IEnumerable? capturedEnumerable = null;
            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, It.IsAny<IEnumerable>()))
                .Callback<Type, object>((_, v) => capturedEnumerable = v as IEnumerable)
                .Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.EqualTo(expectedResult));
                Assert.That(capturedEnumerable, Is.Not.Null);
                Assert.That(capturedEnumerable, Is.Not.SameAs(value));
                Assert.That(capturedEnumerable!.GetType().GetGenericArguments()[0], Is.EqualTo(typeof(object)));
                Assert.That(capturedEnumerable!.GetType().GetGenericArguments()[1], Is.EqualTo(typeof(object)));
            }
        }

        [Test]
        public void Convert_GenericDictionaryValue_DifferentElementTypes_DownCastsElements()
        {
            var targetType = typeof(Dictionary<string, int>);
            var value = new SortedDictionary<object, object> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expectedResult = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };

            IEnumerable? capturedEnumerable = null;
            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, It.IsAny<IEnumerable>()))
                .Callback<Type, object>((_, v) => capturedEnumerable = v as IEnumerable)
                .Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.EqualTo(expectedResult));
                Assert.That(capturedEnumerable, Is.Not.Null);
                Assert.That(capturedEnumerable, Is.Not.SameAs(value));
                Assert.That(capturedEnumerable!.GetType().GetGenericArguments()[0], Is.EqualTo(typeof(string)));
                Assert.That(capturedEnumerable!.GetType().GetGenericArguments()[1], Is.EqualTo(typeof(int)));
            }
        }

        [Test]
        public void Convert_KvpEnumerable_DifferentElementTypes_CastsElements()
        {
            var targetType = typeof(Dictionary<object, object>);
            var value = new List<KeyValuePair<string, int>> { new("one", 1), new("two", 2), new("three", 3) };
            var expectedResult = new Dictionary<object, object> { { "one", 1 }, { "two", 2 }, { "three", 3 } };

            IEnumerable? capturedEnumerable = null;
            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, It.IsAny<IEnumerable>()))
                .Callback<Type, object>((_, v) => capturedEnumerable = v as IEnumerable)
                .Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.EqualTo(expectedResult));
                Assert.That(capturedEnumerable, Is.Not.Null);
                Assert.That(capturedEnumerable, Is.Not.SameAs(value));
                Assert.That(capturedEnumerable!.GetType().GetGenericArguments()[0], Is.EqualTo(typeof(object)));
                Assert.That(capturedEnumerable!.GetType().GetGenericArguments()[1], Is.EqualTo(typeof(object)));
            }
        }

        [Test]
        public void Convert_NonGenericDictionaryValue_CastsToTargetElementType()
        {
            var targetType = typeof(Dictionary<string, int>);
            var value = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expectedResult = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };

            IEnumerable? capturedEnumerable = null;
            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, It.IsAny<IEnumerable>()))
                .Callback<Type, object>((t, v) => capturedEnumerable = v as IEnumerable)
                .Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.EqualTo(expectedResult));
                Assert.That(capturedEnumerable, Is.Not.Null);
                Assert.That(capturedEnumerable, Is.Not.SameAs(value));
                Assert.That(capturedEnumerable!.GetType().GetGenericArguments()[0], Is.EqualTo(typeof(string)));
                Assert.That(capturedEnumerable!.GetType().GetGenericArguments()[1], Is.EqualTo(typeof(int)));
            }
        }

        [Test]
        public void Convert_CannotCastElements_ThrowsException()
        {
            var targetType = typeof(Dictionary<string, long>);
            var value = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };

            var innerMock = new Mock<IValueConverter>();

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            Assert.Throws<InvalidCastException>(() => converter.Convert(targetType, value));
        }
    }
}
