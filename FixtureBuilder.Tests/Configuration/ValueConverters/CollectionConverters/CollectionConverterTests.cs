using System.Collections;
using System.Collections.ObjectModel;
using FixtureBuilder.Configuration.ValueConverters.CollectionConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.CollectionConverters
{
    internal sealed class CollectionConverterTests
    {
        [TestCase(typeof(ReadOnlyCollection<string>))]
        [TestCase(typeof(Collection<string>))]
        public void Convert_ArrayValue_ReturnsCollectionWithSameElements(Type targetType)
        {
            var request = new TypeRequest(targetType, typeof(object));
            string[] value = ["test1", "test2", "test3"];
            var converter = new CollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            var collection = (IEnumerable)result!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(collection, Is.TypeOf(targetType));
                Assert.That(collection, Is.EqualTo(value));
            }
        }

        [TestCase(typeof(ReadOnlyCollection<string>))]
        [TestCase(typeof(Collection<string>))]
        public void Convert_ValueThatIsNotList_ReturnsCollectionWithSameElements(Type targetType)
        {
            var request = new TypeRequest(targetType, typeof(object));
            var value = new Queue<string>(["test1", "test2", "test3"]);
            var converter = new CollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            var collection = (IEnumerable)result!;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(collection, Is.TypeOf(targetType));
                Assert.That(collection, Is.EqualTo(value));
            }
        }

        [TestCase(typeof(ReadOnlyCollection<string>))]
        [TestCase(typeof(Collection<string>))]
        public void Convert_ValueChangedAfterConversion_ResultIsUnchanged(Type targetType)
        {
            var request = new TypeRequest(targetType, typeof(object));
            List<string> value = ["test1", "test2", "test3"];
            var converter = new CollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());
            value.Add("test4");

            var collection = (IEnumerable)result!;
            Assert.That(collection, Is.EqualTo(["test1", "test2", "test3"]));
        }

        [Test]
        public void Convert_ArrayValueToCollection_ResultCanBeAddedTo()
        {
            var request = new TypeRequest(typeof(Collection<string>), typeof(object));
            string[] value = ["test1", "test2", "test3"];
            var converter = new CollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            var collection = (Collection<string>)result!;
            collection.Add("test4");
            Assert.That(collection, Is.EqualTo(["test1", "test2", "test3", "test4"]));
        }

        [Test]
        public void Convert_DifferentElementType_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(Collection<string>), typeof(object));
            int[] value = [1, 2, 3];
            var converter = new CollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_TargetNotCollection_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(List<string>), typeof(object));
            string[] value = ["test1"];
            var converter = new CollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(Collection<string>), typeof(object));
            var converter = new CollectionConverter();

            var result = converter.Convert(request, 42, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericEnumerable_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(Collection<string>), typeof(object));
            var value = new ArrayList { "test1" };
            var converter = new CollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
