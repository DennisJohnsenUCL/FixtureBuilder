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
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new FrozenSetConverter());
        }

        [Test]
        public void Convert_TargetFrozenSet_ValueGenericEnumerable_SameElementType_Converts()
        {
            var target = new FixtureRequest(typeof(FrozenSet<string>));
            var value = new List<string> { "test1", "test2", "test3" };
            var expected = FrozenSet.ToFrozenSet(["test1", "test2", "test3"], null);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new FrozenSetConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetFrozenSet_ValueGenericEnumerable_DifferentElementType_ReturnsNoResult()
        {
            var target = new FixtureRequest(typeof(FrozenSet<string>));
            var value = new List<int> { 1, 2, 3 };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new FrozenSetConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_TargetNotFrozenSet_ReturnsNoResult()
        {
            var target = new FixtureRequest(typeof(List<string>));
            var value = new string[] { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new FrozenSetConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotEnumerable_ReturnsNoResult()
        {
            var target = new FixtureRequest(typeof(FrozenSet<string>));
            var value = 42;
            var context = new Mock<IFixtureContext>().Object;

            var converter = new FrozenSetConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericEnumerable_ReturnsNoResult()
        {
            var target = new FixtureRequest(typeof(FrozenSet<string>));
            var value = new ArrayList { "test1", "test2", "test3" };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new FrozenSetConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
