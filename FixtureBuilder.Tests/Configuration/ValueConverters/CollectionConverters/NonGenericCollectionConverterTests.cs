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
        public void Convert_TargetArrayList_ValueEnumerable_Converts()
        {
            var request = new TypeRequest(typeof(ArrayList), typeof(object));
            var value = new Queue(new ArrayList { "test1", "test2", "test3" });
            var expected = new ArrayList { "test1", "test2", "test3" };
            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetStack_ValueEnumerable_Converts()
        {
            var request = new TypeRequest(typeof(Stack), typeof(object));
            var value = new ArrayList { "test1", "test2", "test3" };
            var expected = new Stack(new ArrayList { "test1", "test2", "test3" });
            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetQueue_ValueEnumerable_Converts()
        {
            var request = new TypeRequest(typeof(Queue), typeof(object));
            var value = new ArrayList { "test1", "test2", "test3" };
            var expected = new Queue(new ArrayList { "test1", "test2", "test3" });
            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetArrayList_ValueGenericEnumerable_Converts()
        {
            var request = new TypeRequest(typeof(ArrayList), typeof(object));
            List<string> value = ["test1", "test2", "test3"];
            var expected = new ArrayList { "test1", "test2", "test3" };
            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetArrayList_ValueNotICollection_Converts()
        {
            var request = new TypeRequest(typeof(ArrayList), typeof(object));
            HashSet<string> value = ["test1", "test2", "test3"];
            ArrayList expected = ["test1", "test2", "test3"];
            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetNotNonGeneric_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(List<string>), typeof(object));
            var value = new ArrayList { "test1", "test2", "test3" };
            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(ArrayList), typeof(object));
            var value = 42;
            var converter = new NonGenericCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
