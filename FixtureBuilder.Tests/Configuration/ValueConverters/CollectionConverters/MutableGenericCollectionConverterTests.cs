using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using FixtureBuilder.Configuration.ValueConverters.CollectionConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.CollectionConverters
{
    internal sealed class MutableGenericCollectionConverterTests
    {
        [Test]
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new MutableGenericCollectionConverter());
        }

        [Test]
        public void Convert_TargetList_Converts()
        {
            var target = typeof(List<string>);
            var value = new string[] { "test1", "test2", "test3" };
            var expected = new List<string> { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetStack_Converts()
        {
            var target = typeof(Stack<string>);
            var value = new string[] { "test1", "test2", "test3" };
            var expected = new Stack<string>(["test1", "test2", "test3"]);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetQueue_Converts()
        {
            var target = typeof(Queue<string>);
            var value = new string[] { "test1", "test2", "test3" };
            var expected = new Queue<string>(["test1", "test2", "test3"]);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetSortedSet_Converts()
        {
            var target = typeof(SortedSet<string>);
            var value = new string[] { "test1", "test2", "test3" };
            var expected = new SortedSet<string> { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetReadOnlyCollection_Converts()
        {
            var target = typeof(ReadOnlyCollection<string>);
            var value = new string[] { "test1", "test2", "test3" };
            var expected = new ReadOnlyCollection<string>(["test1", "test2", "test3"]);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetCollection_Converts()
        {
            var target = typeof(Collection<string>);
            var value = new string[] { "test1", "test2", "test3" };
            var expected = new Collection<string> { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetConcurrentBag_Converts()
        {
            var target = typeof(ConcurrentBag<string>);
            var value = new string[] { "test1", "test2", "test3" };
            var expected = new ConcurrentBag<string> { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetConcurrentStack_Converts()
        {
            var target = typeof(ConcurrentStack<string>);
            var value = new string[] { "test1", "test2", "test3" };
            var expected = new ConcurrentStack<string>(["test1", "test2", "test3"]);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetHashSet_Converts()
        {
            var target = typeof(HashSet<string>);
            var value = new string[] { "test1", "test2", "test3" };
            var expected = new HashSet<string> { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetLinkedList_Converts()
        {
            var target = typeof(LinkedList<string>);
            var value = new string[] { "test1", "test2", "test3" };
            var expected = new LinkedList<string>(["test1", "test2", "test3"]);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetList_ValueNotArray_Converts()
        {
            var target = typeof(List<string>);
            var value = ImmutableList.CreateRange(["test1", "test2", "test3"]);
            var expected = new List<string> { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetList_ValueGenericEnumerable_DifferentElementType_ReturnsNoResult()
        {
            var target = typeof(List<string>);
            var value = new int[] { 1, 2, 3 };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_TargetNotMutableGenericCollection_ReturnsNoResult()
        {
            var target = typeof(ImmutableList<string>);
            var value = new string[] { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var target = typeof(List<string>);
            var value = 42;
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericEnumerable_ReturnsNoResult()
        {
            var target = typeof(List<string>);
            var value = new ArrayList { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
