using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using FixtureBuilder.Configuration.ValueConverters.DictionaryConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.DictionaryConverters
{
    internal sealed class MutableGenericDictionaryConverterTests
    {
        [Test]
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new MutableGenericDictionaryConverter());
        }

        [Test]
        public void Convert_TargetDictionary_ValueGenericDictionary_SameElementType_Converts()
        {
            var target = typeof(Dictionary<string, int>);
            var value = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetDictionary_ValueGenericEnumerableKeyValuePair_SameElementType_Converts()
        {
            var target = typeof(Dictionary<string, int>);
            var value = new List<KeyValuePair<string, int>> { new("one", 1), new("two", 2), new("three", 3) };
            var expected = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetConcurrentDictionary_ValueGenericDictionary_SameElementType_Converts()
        {
            var target = typeof(ConcurrentDictionary<string, int>);
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new ConcurrentDictionary<string, int>(value);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetOrderedDictionary_ValueGenericDictionary_SameElementType_Converts()
        {
            var target = typeof(OrderedDictionary<string, int>);
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new OrderedDictionary<string, int>(value);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetDictionary_ValueGenericDictionary_DifferentElementType_ReturnsNoResult()
        {
            var target = typeof(Dictionary<string, int>);
            var value = new SortedDictionary<string, long> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_TargetNotMutableGenericDictionary_ReturnsNoResult()
        {
            var target = typeof(ReadOnlyDictionary<string, int>);
            var value = new Dictionary<string, long> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericDictionaryOrEnumerableKeyValuePair_ReturnsNoResult()
        {
            var target = typeof(Dictionary<string, int>);
            var value = new List<int>() { 42 };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericDictionary_ReturnsNoResult()
        {
            var target = typeof(Dictionary<string, int>);
            var value = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
