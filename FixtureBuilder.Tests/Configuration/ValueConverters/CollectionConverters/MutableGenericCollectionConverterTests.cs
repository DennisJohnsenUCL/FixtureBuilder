using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using FixtureBuilder.Configuration.ValueConverters.CollectionConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.CollectionConverters
{
    internal sealed class MutableGenericCollectionConverterTests
    {
        [TestCase(typeof(List<string>))]
        [TestCase(typeof(Queue<string>))]
        [TestCase(typeof(SortedSet<string>))]
        [TestCase(typeof(HashSet<string>))]
        [TestCase(typeof(LinkedList<string>))]
        public void Convert_OrderPreservingTarget_ReturnsCollectionWithElementsInOrder(Type targetType)
        {
            var request = new TypeRequest(targetType, typeof(object));
            string[] value = ["test1", "test2", "test3"];
            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            var collection = (IEnumerable)result!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(collection, Is.TypeOf(targetType));
                Assert.That(collection, Is.EqualTo(["test1", "test2", "test3"]));
            }
        }

        [TestCase(typeof(Stack<string>))]
        [TestCase(typeof(ConcurrentStack<string>))]
        [TestCase(typeof(ConcurrentBag<string>))]
        public void Convert_ReverseOrderTarget_ReturnsCollectionWithElementsReversed(Type targetType)
        {
            var request = new TypeRequest(targetType, typeof(object));
            string[] value = ["test1", "test2", "test3"];
            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            var collection = (IEnumerable)result!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(collection, Is.TypeOf(targetType));
                Assert.That(collection, Is.EqualTo(["test3", "test2", "test1"]));
            }
        }

        [Test]
        public void Convert_ValueThatIsNotArray_ReturnsList()
        {
            var request = new TypeRequest(typeof(List<string>), typeof(object));
            ImmutableList<string> value = ["test1", "test2", "test3"];
            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(["test1", "test2", "test3"]));
        }

        [Test]
        public void Convert_DifferentElementType_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(List<string>), typeof(object));
            int[] value = [1, 2, 3];
            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_TargetNotMutableGenericCollection_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(ImmutableList<string>), typeof(object));
            string[] value = ["test1"];
            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(List<string>), typeof(object));
            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(request, 42, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericEnumerable_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(List<string>), typeof(object));
            var value = new ArrayList { "test1" };
            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
