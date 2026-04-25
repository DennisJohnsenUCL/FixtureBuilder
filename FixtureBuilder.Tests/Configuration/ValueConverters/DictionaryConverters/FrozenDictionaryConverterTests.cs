using System.Collections;
using System.Collections.Frozen;
using System.Collections.ObjectModel;
using FixtureBuilder.Configuration.ValueConverters.DictionaryConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.DictionaryConverters
{
    internal sealed class FrozenDictionaryConverterTests
    {
        [Test]
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new FrozenDictionaryConverter());
        }

        [Test]
        public void Convert_TargetFrozenDictionary_ValueGenericDictionary_SameElementType_Converts()
        {
            var target = new FixtureRequest(typeof(FrozenDictionary<string, int>));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = FrozenDictionary.ToFrozenDictionary(value, null);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetFrozenDictionary_ValueGenericEnumerableKeyValuePair_SameElementType_Converts()
        {
            var target = new FixtureRequest(typeof(FrozenDictionary<string, int>));
            var value = new List<KeyValuePair<string, int>> { new("one", 1), new("two", 2), new("three", 3) };
            var expected = FrozenDictionary.ToFrozenDictionary(value, null);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetFrozenDictionary_ValueGenericDictionary_DifferentElementType_ReturnsNoResult()
        {
            var target = new FixtureRequest(typeof(FrozenDictionary<string, int>));
            var value = new Dictionary<string, long> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_TargetNotFrozenDictionary_ReturnsNoResult()
        {
            var target = new FixtureRequest(typeof(ReadOnlyDictionary<string, int>));
            var value = new Dictionary<string, long> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericDictionaryOrEnumerableKeyValuePair_ReturnsNoResult()
        {
            var target = new FixtureRequest(typeof(FrozenDictionary<string, int>));
            var value = new List<int>() { 42 };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericDictionary_ReturnsNoResult()
        {
            var target = new FixtureRequest(typeof(FrozenDictionary<string, int>));
            var value = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
