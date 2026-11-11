using System.Collections;
using FixtureBuilder.Configuration.ValueConverters.CollectionConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.CollectionConverters
{
    internal sealed class ArrayConverterTests
    {
        [Test]
        public void Convert_TargetArray_ValueEnumerable_Converts()
        {
            var request = new TypeRequest(typeof(string[]), typeof(object));
            var value = new ArrayList { "test1", "test2", "test3" };
            string[] expected = ["test1", "test2", "test3"];
            var converter = new ArrayConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetArray_ValueGenericEnumerable_Converts()
        {
            var request = new TypeRequest(typeof(string[]), typeof(object));
            List<string> value = ["test1", "test2", "test3"];
            string[] expected = ["test1", "test2", "test3"];
            var converter = new ArrayConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetNotArray_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(List<string>), typeof(object));
            var value = new ArrayList { "test1", "test2", "test3" };
            var converter = new ArrayConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(string[]), typeof(object));
            const int value = 42;
            var converter = new ArrayConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
