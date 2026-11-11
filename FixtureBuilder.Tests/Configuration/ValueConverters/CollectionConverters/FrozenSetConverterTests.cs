using System.Collections;
using System.Collections.Frozen;
using FixtureBuilder.Configuration.ValueConverters.CollectionConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.CollectionConverters
{
    internal sealed class FrozenSetConverterTests
    {
        [Test]
        public void Convert_TargetFrozenSet_ValueGenericEnumerable_SameElementType_Converts()
        {
            var request = new TypeRequest(typeof(FrozenSet<string>), typeof(object));
            List<string> value = ["test1", "test2", "test3"];
            var expected = FrozenSet.ToFrozenSet(["test1", "test2", "test3"], null);
            var converter = new FrozenSetConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetFrozenSet_ValueGenericEnumerable_DifferentElementType_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(FrozenSet<string>), typeof(object));
            List<int> value = [1, 2, 3];
            var converter = new FrozenSetConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_TargetNotFrozenSet_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(List<string>), typeof(object));
            string[] value = ["test1", "test2", "test3"];
            var converter = new FrozenSetConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(FrozenSet<string>), typeof(object));
            var value = 42;
            var converter = new FrozenSetConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericEnumerable_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(FrozenSet<string>), typeof(object));
            var value = new ArrayList { "test1", "test2", "test3" };
            var converter = new FrozenSetConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
