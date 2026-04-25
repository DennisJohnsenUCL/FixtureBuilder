using System.Collections;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.Decorators;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.Decorators
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
            var targetType = new FixtureRequest(typeof(Dictionary<string, int>));
            var value = 5;
            var expectedResult = "converted";
            var context = new Mock<IFixtureContext>().Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value, context)).Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value, context), Times.Once);
        }

        [Test]
        public void Convert_ValueNotDictionaryOrEnumerableKvp_PassesThroughToInner()
        {
            var targetType = new FixtureRequest(typeof(Dictionary<string, int>));
            var value = new List<string> { "one", "two", "three" };
            var expectedResult = "converted";
            var context = new Mock<IFixtureContext>().Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value, context)).Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value, context), Times.Once);
        }

        [Test]
        public void Convert_TargetTypeNotDictionary_PassesThroughToInner()
        {
            var targetType = new FixtureRequest(typeof(List<string>));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expectedResult = "converted";
            var context = new Mock<IFixtureContext>().Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value, context)).Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value, context), Times.Once);
        }

        [Test]
        public void Convert_NonGenericDictionaryTargetType_PassesThroughToInner()
        {
            var targetType = new FixtureRequest(typeof(Hashtable));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expectedResult = "converted";
            var context = new Mock<IFixtureContext>().Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value, context)).Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value, context), Times.Once);
        }

        [Test]
        public void Convert_SameElementTypes_PassesThroughToInner()
        {
            var targetType = new FixtureRequest(typeof(Dictionary<string, int>));
            var value = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expectedResult = "converted";
            var context = new Mock<IFixtureContext>().Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value, context)).Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value, context), Times.Once);
        }

        [Test]
        public void Convert_GenericDictionaryValue_DifferentElementTypes_CastsElements()
        {
            var targetType = new FixtureRequest(typeof(Dictionary<object, object>));
            var value = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expectedResult = new Dictionary<object, object> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            IEnumerable? capturedEnumerable = null;
            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, It.IsAny<IEnumerable>(), context))
                .Callback<FixtureRequest, object, IFixtureContext>((_, v, _) => capturedEnumerable = v as IEnumerable)
                .Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

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
            var targetType = new FixtureRequest(typeof(Dictionary<string, int>));
            var value = new SortedDictionary<object, object> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expectedResult = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            IEnumerable? capturedEnumerable = null;
            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, It.IsAny<IEnumerable>(), context))
                .Callback<FixtureRequest, object, IFixtureContext>((_, v, _) => capturedEnumerable = v as IEnumerable)
                .Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

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
            var targetType = new FixtureRequest(typeof(Dictionary<object, object>));
            var value = new List<KeyValuePair<string, int>> { new("one", 1), new("two", 2), new("three", 3) };
            var expectedResult = new Dictionary<object, object> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            IEnumerable? capturedEnumerable = null;
            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, It.IsAny<IEnumerable>(), context))
                .Callback<FixtureRequest, object, IFixtureContext>((_, v, _) => capturedEnumerable = v as IEnumerable)
                .Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

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
            var targetType = new FixtureRequest(typeof(Dictionary<string, int>));
            var value = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expectedResult = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            IEnumerable? capturedEnumerable = null;
            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, It.IsAny<IEnumerable>(), context))
                .Callback<FixtureRequest, object, IFixtureContext>((_, v, _) => capturedEnumerable = v as IEnumerable)
                .Returns(expectedResult);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

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
            var targetType = new FixtureRequest(typeof(Dictionary<string, long>));
            var value = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };

            var rootConverter = new Mock<IValueConverter>();
            rootConverter.Setup(c => c.Convert(It.IsAny<FixtureRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()))
                .Returns(new NoResult());

            var composite = new Mock<ICompositeConverter>();
            var converterGraph = new ConverterGraph(rootConverter.Object, composite.Object);

            var context = new Mock<IFixtureContext>();
            context.Setup(c => c.Converter).Returns(converterGraph);

            var innerMock = new Mock<IValueConverter>();

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            Assert.Throws<InvalidCastException>(() => converter.Convert(targetType, value, context.Object));
        }

        [Test]
        public void Convert_ElementsNotAssignable_ConvertsViaRoot()
        {
            var targetType = new FixtureRequest(typeof(Dictionary<string, long>));
            var value = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };

            var rootConverter = new Mock<IValueConverter>();
            rootConverter.Setup(c => c.Convert(It.Is<FixtureRequest>(r => r.Type == typeof(long)), It.IsAny<object>(), It.IsAny<IFixtureContext>()))
                .Returns((FixtureRequest r, object v, IFixtureContext c) => System.Convert.ChangeType(v, r.Type));

            var composite = new Mock<ICompositeConverter>();
            var converterGraph = new ConverterGraph(rootConverter.Object, composite.Object);

            var context = new Mock<IFixtureContext>();
            context.Setup(c => c.Converter).Returns(converterGraph);

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(c => c.Convert(It.IsAny<FixtureRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()))
                .Returns((FixtureRequest r, object v, IFixtureContext c) => v);

            var converter = new DictionaryElementCastingConverter(innerMock.Object);

            var result = (Dictionary<string, long>)converter.Convert(targetType, value, context.Object)!;

            Assert.That(result, Is.EqualTo(new Dictionary<string, long> { { "one", 1L }, { "two", 2L }, { "three", 3L } }));
        }
    }
}
