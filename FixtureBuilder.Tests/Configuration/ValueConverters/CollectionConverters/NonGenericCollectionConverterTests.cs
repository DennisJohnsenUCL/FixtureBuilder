using System.Collections;
using FixtureBuilder.Configuration.ValueConverters.CollectionConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.CollectionConverters
{
    internal sealed class NonGenericCollectionConverterTests
    {
        [Test]
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new MutableGenericCollectionConverter());
        }

        [Test]
        public void Convert_TargetArrayList_ValueEnumerable_Converts()
        {
            var target = new FixtureRequest(typeof(ArrayList));
            var value = new Queue(new ArrayList { "test1", "test2", "test3" });
            var expected = new ArrayList { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetStack_ValueEnumerable_Converts()
        {
            var target = new FixtureRequest(typeof(Stack));
            var value = new ArrayList { "test1", "test2", "test3" };
            var expected = new Stack(new ArrayList { "test1", "test2", "test3" });
            var context = new Mock<IFixtureContext>().Object;

            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetQueue_ValueEnumerable_Converts()
        {
            var target = new FixtureRequest(typeof(Queue));
            var value = new ArrayList { "test1", "test2", "test3" };
            var expected = new Queue(new ArrayList { "test1", "test2", "test3" });
            var context = new Mock<IFixtureContext>().Object;

            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetArrayList_ValueGenericEnumerable_Converts()
        {
            var target = new FixtureRequest(typeof(ArrayList));
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = new ArrayList { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetArrayList_ValueNotICollection_Converts()
        {
            var target = new FixtureRequest(typeof(ArrayList));
            var value = new HashSet<string> { "test1", "test2", "test3" };
            var expected = new ArrayList { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetNotNonGeneric_ReturnsNoResult()
        {
            var target = new FixtureRequest(typeof(List<string>));
            var value = new ArrayList { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var target = new FixtureRequest(typeof(ArrayList));
            var value = 42;
            var context = new Mock<IFixtureContext>().Object;

            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
