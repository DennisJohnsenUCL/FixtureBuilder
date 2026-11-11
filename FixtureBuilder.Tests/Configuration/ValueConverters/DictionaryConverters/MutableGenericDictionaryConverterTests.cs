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
        public void Convert_TargetDictionary_ValueGenericDictionary_SameElementType_Converts()
        {
            var request = new TypeRequest(typeof(Dictionary<string, int>), typeof(object));
            var value = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetDictionary_ValueGenericEnumerableKeyValuePair_SameElementType_Converts()
        {
            var request = new TypeRequest(typeof(Dictionary<string, int>), typeof(object));
            List<KeyValuePair<string, int>> value = [new("one", 1), new("two", 2), new("three", 3)];
            var expected = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetConcurrentDictionary_ValueGenericDictionary_SameElementType_Converts()
        {
            var request = new TypeRequest(typeof(ConcurrentDictionary<string, int>), typeof(object));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new ConcurrentDictionary<string, int>(value);
            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetOrderedDictionary_ValueGenericDictionary_SameElementType_Converts()
        {
            var request = new TypeRequest(typeof(OrderedDictionary<string, int>), typeof(object));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new OrderedDictionary<string, int>(value);
            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetDictionary_ValueGenericDictionary_DifferentElementType_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(Dictionary<string, int>), typeof(object));
            var value = new SortedDictionary<string, long> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_TargetNotMutableGenericDictionary_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(ReadOnlyDictionary<string, int>), typeof(object));
            var value = new Dictionary<string, long> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericDictionaryOrEnumerableKeyValuePair_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(Dictionary<string, int>), typeof(object));
            List<int> value = [42];
            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericDictionary_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(Dictionary<string, int>), typeof(object));
            var value = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new MutableGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
