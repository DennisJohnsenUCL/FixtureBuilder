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
        public void Convert_TargetImmutableList_Converts()
        {
            var request = new TypeRequest(typeof(ImmutableList<string>), typeof(object));
            List<string> value = ["test1", "test2", "test3"];
            ImmutableList<string> expected = ["test1", "test2", "test3"];
            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableHashSet_Converts()
        {
            var request = new TypeRequest(typeof(ImmutableHashSet<string>), typeof(object));
            List<string> value = ["test1", "test2", "test3"];
            ImmutableHashSet<string> expected = ["test1", "test2", "test3"];
            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableStack_Converts()
        {
            var request = new TypeRequest(typeof(ImmutableStack<string>), typeof(object));
            List<string> value = ["test1", "test2", "test3"];
            ImmutableStack<string> expected = ["test1", "test2", "test3"];
            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableQueue_Converts()
        {
            var request = new TypeRequest(typeof(ImmutableQueue<string>), typeof(object));
            List<string> value = ["test1", "test2", "test3"];
            ImmutableQueue<string> expected = ["test1", "test2", "test3"];
            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableArray_Converts()
        {
            var request = new TypeRequest(typeof(ImmutableArray<string>), typeof(object));
            List<string> value = ["test1", "test2", "test3"];
            ImmutableArray<string> expected = ["test1", "test2", "test3"];
            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableSortedSet_Converts()
        {
            var request = new TypeRequest(typeof(ImmutableSortedSet<string>), typeof(object));
            List<string> value = ["test1", "test2", "test3"];
            ImmutableSortedSet<string> expected = ["test1", "test2", "test3"];
            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableList_ValueGenericEnumerable_DifferentElementType_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(ImmutableList<string>), typeof(object));
            List<int> value = [1, 2, 3];
            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_TargetNotImmutable_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(List<string>), typeof(object));
            string[] value = ["test1", "test2", "test3"];
            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(ImmutableList<string>), typeof(object));
            var value = 42;
            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericEnumerable_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(ImmutableList<string>), typeof(object));
            var value = new ArrayList { "test1", "test2", "test3" };
            var converter = new ImmutableCollectionConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
