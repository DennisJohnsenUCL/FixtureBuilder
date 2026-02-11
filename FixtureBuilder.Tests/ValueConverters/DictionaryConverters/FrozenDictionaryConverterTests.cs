using FixtureBuilder.ValueConverters.DictionaryConverters;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.ObjectModel;

namespace FixtureBuilder.Tests.ValueConverters.DictionaryConverters
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
            var target = typeof(FrozenDictionary<string, int>);
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = FrozenDictionary.ToFrozenDictionary(value, null);

            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetFrozenDictionary_ValueGenericEnumerableKeyValuePair_SameElementType_Converts()
        {
            var target = typeof(FrozenDictionary<string, int>);
            var value = new List<KeyValuePair<string, int>> { new("one", 1), new("two", 2), new("three", 3) };
            var expected = FrozenDictionary.ToFrozenDictionary(value, null);

            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetFrozenDictionary_ValueGenericDictionary_DifferentElementType_ReturnsNull()
        {
            var target = typeof(FrozenDictionary<string, int>);
            var value = new Dictionary<string, long> { { "one", 1 }, { "two", 2 }, { "three", 3 } };

            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_TargetNotFrozenDictionary_ReturnsNull()
        {
            var target = typeof(ReadOnlyDictionary<string, int>);
            var value = new Dictionary<string, long> { { "one", 1 }, { "two", 2 }, { "three", 3 } };

            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_ValueNotGenericDictionaryOrEnumerableKeyValuePair_ReturnsNull()
        {
            var target = typeof(FrozenDictionary<string, int>);
            var value = new List<int>() { 42 };

            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_ValueNotGenericDictionary_ReturnsNull()
        {
            var target = typeof(FrozenDictionary<string, int>);
            var value = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };

            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(target, value);

            Assert.That(result, Is.Null);
        }
    }
}
