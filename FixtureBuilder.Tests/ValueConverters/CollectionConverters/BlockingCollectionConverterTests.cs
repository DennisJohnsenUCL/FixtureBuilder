using System.Collections;
using System.Collections.Concurrent;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.ValueConverters.CollectionConverters;
using Moq;

namespace FixtureBuilder.Tests.ValueConverters.CollectionConverters
{
    internal sealed class BlockingCollectionConverterTests
    {
        [Test]
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new BlockingCollectionConverter());
        }

        [Test]
        public void Convert_TargetBlockingCollection_ValueGenericEnumerable_SameElementType_Converts()
        {
            var target = typeof(BlockingCollection<string>);
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = new BlockingCollection<string> { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetBlockingCollection_ValueIProducerConsumerCollection_Converts()
        {
            var target = typeof(BlockingCollection<string>);
            var value = new ConcurrentQueue<string>(["test1", "test2", "test3"]);
            var expected = new BlockingCollection<string> { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetBlockingCollection_ValueGenericEnumerable_DifferentElementType_ReturnsNoResult()
        {
            var target = typeof(BlockingCollection<string>);
            var value = new List<int> { 1, 2, 3 };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_TargetNotBlockingCollection_ReturnsNoResult()
        {
            var target = typeof(List<string>);
            var value = new string[] { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var target = typeof(BlockingCollection<string>);
            var value = 42;
            var context = new Mock<IFixtureContext>().Object;

            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericEnumerable_ReturnsNoResult()
        {
            var target = typeof(BlockingCollection<string>);
            var value = new ArrayList { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
