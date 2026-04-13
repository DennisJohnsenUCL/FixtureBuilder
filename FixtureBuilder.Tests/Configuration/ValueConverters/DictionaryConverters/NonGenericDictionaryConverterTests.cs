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
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new NonGenericDictionaryConverter());
        }

        [Test]
        public void Convert_TargetHashtable_ValueGenericDictionary_Converts()
        {
            var target = typeof(Hashtable);
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new NonGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetHashtable_ValueNonGenericDictionary_Converts()
        {
            var target = typeof(Hashtable);
            var value = new SortedList { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new NonGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetHashTable_ValueGenericEnumerableKeyValuePair_Converts()
        {
            var target = typeof(Hashtable);
            var value = new List<KeyValuePair<string, int>> { new("one", 1), new("two", 2), new("three", 3) };
            var expected = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new NonGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetSortedList_ValueGenericDictionary_Converts()
        {
            var target = typeof(SortedList);
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new SortedList { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new NonGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetNotNonGenericDictionary_ReturnsNoResult()
        {
            var target = typeof(Dictionary<string, int>);
            var value = new SortedList { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new NonGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericDictionaryOrEnumerableKeyValuePair_ReturnsNoResult()
        {
            var target = typeof(Hashtable);
            var value = new List<int>() { 42 };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new NonGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
