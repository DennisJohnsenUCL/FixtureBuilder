using FixtureBuilder.ValueConverters.Converters;
using System.Collections;
using System.Collections.Immutable;

namespace FixtureBuilder.Tests.ValueConverters.Converters
{
    internal sealed class ImmutableCollectionConverterTests
    {
        [Test]
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new ImmutableCollectionConverter());
        }

        [Test]
        public void Convert_TargetImmutableList_Converts()
        {
            var target = typeof(ImmutableList<string>);
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = ImmutableList.CreateRange(["test1", "test2", "test3"]);

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableHashSet_Converts()
        {
            var target = typeof(ImmutableHashSet<string>);
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = ImmutableHashSet.CreateRange(["test1", "test2", "test3"]);

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableStack_Converts()
        {
            var target = typeof(ImmutableStack<string>);
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = ImmutableStack.CreateRange(["test1", "test2", "test3"]);

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableQueue_Converts()
        {
            var target = typeof(ImmutableQueue<string>);
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = ImmutableQueue.CreateRange(["test1", "test2", "test3"]);

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableArray_Converts()
        {
            var target = typeof(ImmutableArray<string>);
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = ImmutableArray.CreateRange(["test1", "test2", "test3"]);

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableSortedSet_Converts()
        {
            var target = typeof(ImmutableSortedSet<string>);
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = ImmutableSortedSet.CreateRange(["test1", "test2", "test3"]);

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableList_ValueGenericEnumerable_DifferentElementType_ReturnsNull()
        {
            var target = typeof(ImmutableList<string>);
            var value = new List<int> { 1, 2, 3 };

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_TargetNotImmutable_ReturnsNull()
        {
            var target = typeof(List<string>);
            var value = new string[] { "test1", "test2", "test3" };

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNull()
        {
            var target = typeof(ImmutableList<string>);
            var value = 42;

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_ValueNotGenericEnumerable_ReturnsNull()
        {
            var target = typeof(ImmutableList<string>);
            var value = new ArrayList { "test1", "test2", "test3" };

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.Null);
        }
    }
}
