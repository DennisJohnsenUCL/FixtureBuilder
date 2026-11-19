using System.Collections.Concurrent;
using FixtureBuilder.Configuration.ValueConverters.CollectionConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.CollectionConverters
{
    internal sealed class BlockingCollectionConverterTests
    {
        [Test]
        public void Convert_TargetBlockingCollection_ValueGenericEnumerable_SameElementType_Converts()
        {
            var request = TestHelper.MakeConverterRequest(typeof(BlockingCollection<string>));
            List<string> value = ["test1", "test2", "test3"];
            var expected = new BlockingCollection<string> { "test1", "test2", "test3" };
            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetBlockingCollection_ValueIProducerConsumerCollection_Converts()
        {
            var request = TestHelper.MakeConverterRequest(typeof(BlockingCollection<string>));
            var value = new ConcurrentQueue<string>(["test1", "test2", "test3"]);
            var expected = new BlockingCollection<string> { "test1", "test2", "test3" };
            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetNotBlockingCollection_ReturnsNoResult()
        {
            var request = TestHelper.MakeConverterRequest(typeof(List<string>));
            string[] value = ["test1", "test2", "test3"];
            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var request = TestHelper.MakeConverterRequest(typeof(BlockingCollection<string>));
            const int value = 42;
            var converter = new BlockingCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
