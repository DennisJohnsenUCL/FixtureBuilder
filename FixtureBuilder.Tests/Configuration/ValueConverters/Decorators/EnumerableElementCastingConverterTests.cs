using System.Collections;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.Decorators;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.Decorators
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
            var context = new Mock<IFixtureContext>().Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value, context)).Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value, context), Times.Once);
        }

        [Test]
        public void Convert_NonEnumerableTargetType_PassesThroughToInner()
        {
            var targetType = typeof(long);
            var value = new List<int> { 1, 2, 3 };
            var expectedResult = "converted";
            var context = new Mock<IFixtureContext>().Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value, context)).Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value, context), Times.Once);
        }

        [Test]
        public void Convert_StringTargetType_PassesThroughToInner()
        {
            var targetType = typeof(string);
            var value = new List<int> { 1, 2, 3 };
            var expectedResult = "converted";
            var context = new Mock<IFixtureContext>().Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value, context)).Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value, context), Times.Once);
        }

        [Test]
        public void Convert_ArrayTargetType_PassesThroughToInner()
        {
            var targetType = typeof(int[]);
            var value = new List<int> { 1, 2, 3 };
            var expectedResult = new int[] { 1, 2, 3 };
            var context = new Mock<IFixtureContext>().Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value, context)).Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value, context), Times.Once);
        }

        [Test]
        public void Convert_DictionaryTargetType_PassesThroughToInner()
        {
            var targetType = typeof(Dictionary<,>);
            var value = new List<int> { 1, 2, 3 };
            var expectedResult = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value, context)).Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value, context), Times.Once);
        }

        [Test]
        public void Convert_SameElementTypes_PassesThroughToInner()
        {
            var targetType = typeof(Stack<int>);
            var value = new List<int> { 1, 2, 3 };
            var expectedResult = "converted";
            var context = new Mock<IFixtureContext>().Object;

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, value, context)).Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

            Assert.That(result, Is.EqualTo(expectedResult));
            innerMock.Verify(x => x.Convert(targetType, value, context), Times.Once);
        }

        [Test]
        public void Convert_DifferentElementTypes_CastsElementsBeforePassingToInner()
        {
            var targetType = typeof(List<object>);
            var value = new List<int> { 1, 2, 3 };
            var expectedResult = new List<object> { 1, 2, 3 };
            var context = new Mock<IFixtureContext>().Object;

            IEnumerable? capturedEnumerable = null;
            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, It.IsAny<IEnumerable>(), context))
                .Callback<Type, object, IFixtureContext>((_, v, _) => capturedEnumerable = v as IEnumerable)
                .Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

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
            var context = new Mock<IFixtureContext>().Object;

            IEnumerable? capturedEnumerable = null;
            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(x => x.Convert(targetType, It.IsAny<IEnumerable>(), context))
                .Callback<Type, object, IFixtureContext>((_, v, _) => capturedEnumerable = v as IEnumerable)
                .Returns(expectedResult);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = converter.Convert(targetType, value, context);

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
            var context = new Mock<IFixtureContext>();
            var rootConverter = new Mock<IValueConverter>();
            rootConverter.Setup(c => c.Convert(It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()))
                .Returns(new NoResult());

            var composite = new Mock<ICompositeConverter>();
            var converterGraph = new ConverterGraph(rootConverter.Object, composite.Object);

            context.Setup(c => c.Converter).Returns(converterGraph);

            var innerMock = new Mock<IValueConverter>();

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            Assert.Throws<InvalidCastException>(() => converter.Convert(targetType, value, context.Object));
        }

        [Test]
        public void Convert_ElementsNotAssignable_ConvertsViaRoot()
        {
            var targetType = typeof(List<long>);
            var value = new List<int> { 1, 2, 3 };

            var rootConverter = new Mock<IValueConverter>();
            rootConverter.Setup(c => c.Convert(typeof(long), It.IsAny<object>(), It.IsAny<IFixtureContext>()))
                .Returns((Type t, object v, IFixtureContext c) => System.Convert.ChangeType(v, t));

            var composite = new Mock<ICompositeConverter>();
            var converterGraph = new ConverterGraph(rootConverter.Object, composite.Object);

            var context = new Mock<IFixtureContext>();
            context.Setup(c => c.Converter).Returns(converterGraph);

            var innerMock = new Mock<IValueConverter>();
            innerMock.Setup(c => c.Convert(It.IsAny<Type>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()))
                .Returns((Type t, object v, IFixtureContext c) => v);

            var converter = new EnumerableElementCastingConverter(innerMock.Object);

            var result = (List<long>)converter.Convert(targetType, value, context.Object)!;

            Assert.That(result, Is.EqualTo(new List<long> { 1L, 2L, 3L }));
        }
    }
}
