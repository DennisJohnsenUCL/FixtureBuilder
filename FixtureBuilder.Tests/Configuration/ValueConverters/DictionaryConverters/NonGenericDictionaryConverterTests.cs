using System.Collections;
using FixtureBuilder.Configuration.ValueConverters.DictionaryConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.DictionaryConverters
{
    internal sealed class NonGenericDictionaryConverterTests
    {
        [Test]
        public void Convert_TargetHashtable_ValueGenericDictionary_Converts()
        {
            var request = new TypeRequest(typeof(Hashtable), typeof(object));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new NonGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetHashtable_ValueNonGenericDictionary_Converts()
        {
            var request = new TypeRequest(typeof(Hashtable), typeof(object));
            var value = new SortedList { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new NonGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetHashTable_ValueGenericEnumerableKeyValuePair_Converts()
        {
            var request = new TypeRequest(typeof(Hashtable), typeof(object));
            List<KeyValuePair<string, int>> value = [new("one", 1), new("two", 2), new("three", 3)];
            var expected = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new NonGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetSortedList_ValueGenericDictionary_Converts()
        {
            var request = new TypeRequest(typeof(SortedList), typeof(object));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new SortedList { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new NonGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetNotNonGenericDictionary_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(Dictionary<string, int>), typeof(object));
            var value = new SortedList { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new NonGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericDictionaryOrEnumerableKeyValuePair_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(Hashtable), typeof(object));
            List<int> value = [42];
            var converter = new NonGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
