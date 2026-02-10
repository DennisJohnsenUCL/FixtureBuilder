using FixtureBuilder.ValueConverters.Converters;
using System.Collections;
using System.Collections.Concurrent;

namespace FixtureBuilder.Tests.ValueConverters.Converters
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

            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetBlockingCollection_ValueIProducerConsumerCollection_Converts()
        {
            var target = typeof(BlockingCollection<string>);
            var value = new ConcurrentQueue<string>(["test1", "test2", "test3"]);
            var expected = new BlockingCollection<string> { "test1", "test2", "test3" };

            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetBlockingCollection_ValueGenericEnumerable_DifferentElementType_ReturnsNull()
        {
            var target = typeof(BlockingCollection<string>);
            var value = new List<int> { 1, 2, 3 };

            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_TargetNotBlockingCollection_ReturnsNull()
        {
            var target = typeof(List<string>);
            var value = new string[] { "test1", "test2", "test3" };

            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNull()
        {
            var target = typeof(BlockingCollection<string>);
            var value = 42;

            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_ValueNotGenericEnumerable_ReturnsNull()
        {
            var target = typeof(BlockingCollection<string>);
            var value = new ArrayList { "test1", "test2", "test3" };

            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.Null);
        }
    }
}
