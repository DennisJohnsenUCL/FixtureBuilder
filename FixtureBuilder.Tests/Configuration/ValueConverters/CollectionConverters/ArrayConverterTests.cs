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
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new ArrayConverter());
        }

        [Test]
        public void Convert_TargetArray_ValueEnumerable_Converts()
        {
            var target = new FixtureRequest(typeof(string[]));
            var value = new ArrayList { "test1", "test2", "test3" };
            var expected = new string[] { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ArrayConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetArray_ValueGenericEnumerable_Converts()
        {
            var target = new FixtureRequest(typeof(string[]));
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = new string[] { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ArrayConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetNotArray_ReturnsNoResult()
        {
            var target = new FixtureRequest(typeof(List<string>));
            var value = new ArrayList { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ArrayConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var target = new FixtureRequest(typeof(string[]));
            var value = 42;
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ArrayConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
