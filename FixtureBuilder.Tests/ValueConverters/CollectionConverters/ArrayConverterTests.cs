using FixtureBuilder.FixtureContexts;
using FixtureBuilder.ValueConverters.CollectionConverters;
using Moq;
using System.Collections;

namespace FixtureBuilder.Tests.ValueConverters.CollectionConverters
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
            var target = typeof(string[]);
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
            var target = typeof(string[]);
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = new string[] { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ArrayConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetNotArray_ReturnsNull()
        {
            var target = typeof(List<string>);
            var value = new ArrayList { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ArrayConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNull()
        {
            var target = typeof(string[]);
            var value = 42;
            var context = new Mock<IFixtureContext>().Object;

            var converter = new ArrayConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.Null);
        }
    }
}
