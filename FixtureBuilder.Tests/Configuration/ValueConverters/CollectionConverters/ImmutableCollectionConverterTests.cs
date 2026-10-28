using System.Collections;
using System.Collections.Immutable;
using FixtureBuilder.Configuration.ValueConverters.CollectionConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.CollectionConverters
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
            var target = new TypeRequest(typeof(ImmutableList<string>), typeof(ImmutableList<string>));
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = ImmutableList.CreateRange(["test1", "test2", "test3"]);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableHashSet_Converts()
        {
            var target = new TypeRequest(typeof(ImmutableHashSet<string>), typeof(ImmutableHashSet<string>));
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = ImmutableHashSet.CreateRange(["test1", "test2", "test3"]);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableStack_Converts()
        {
            var target = new TypeRequest(typeof(ImmutableStack<string>), typeof(ImmutableStack<string>));
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = ImmutableStack.CreateRange(["test1", "test2", "test3"]);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableQueue_Converts()
        {
            var target = new TypeRequest(typeof(ImmutableQueue<string>), typeof(ImmutableQueue<string>));
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = ImmutableQueue.CreateRange(["test1", "test2", "test3"]);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableArray_Converts()
        {
            var target = new TypeRequest(typeof(ImmutableArray<string>), typeof(ImmutableArray<string>));
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = ImmutableArray.CreateRange(["test1", "test2", "test3"]);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableSortedSet_Converts()
        {
            var target = new TypeRequest(typeof(ImmutableSortedSet<string>), typeof(ImmutableSortedSet<string>));
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = ImmutableSortedSet.CreateRange(["test1", "test2", "test3"]);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableList_ValueGenericEnumerable_DifferentElementType_ReturnsNoResult()
        {
            var target = new TypeRequest(typeof(ImmutableList<string>), typeof(ImmutableList<string>));
            var value = new List<int> { 1, 2, 3 };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_TargetNotImmutable_ReturnsNoResult()
        {
            var target = new TypeRequest(typeof(List<string>), typeof(List<string>));
            var value = new string[] { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var target = new TypeRequest(typeof(ImmutableList<string>), typeof(ImmutableList<string>));
            var value = 42;
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericEnumerable_ReturnsNoResult()
        {
            var target = new TypeRequest(typeof(ImmutableList<string>), typeof(ImmutableList<string>));
            var value = new ArrayList { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
