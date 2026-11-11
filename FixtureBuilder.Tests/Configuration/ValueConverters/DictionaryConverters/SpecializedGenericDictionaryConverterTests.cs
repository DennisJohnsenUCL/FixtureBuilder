using System.Collections;
using System.Collections.ObjectModel;
using FixtureBuilder.Configuration.ValueConverters.DictionaryConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.DictionaryConverters
{
    internal sealed class SpecializedGenericDictionaryConverterTests
    {
        [Test]
        public void Convert_TargetReadOnlyDictionary_ValueGenericDictionary_SameElementType_Converts()
        {
            var request = new TypeRequest(typeof(ReadOnlyDictionary<string, int>), typeof(object));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new ReadOnlyDictionary<string, int>(value);
            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetReadOnlyDictionary_ValueGenericEnumerableKeyValuePair_SameElementType_Converts()
        {
            var request = new TypeRequest(typeof(ReadOnlyDictionary<string, int>), typeof(object));
            List<KeyValuePair<string, int>> value = [new("one", 1), new("two", 2), new("three", 3)];
            var expected = new ReadOnlyDictionary<string, int>(new Dictionary<string, int>(value));
            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetSortedDictionary_ValueGenericDictionary_SameElementType_Converts()
        {
            var request = new TypeRequest(typeof(SortedDictionary<string, int>), typeof(object));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetSortedList_ValueGenericDictionary_SameElementType_Converts()
        {
            var request = new TypeRequest(typeof(SortedList<string, int>), typeof(object));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new SortedList<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetReadOnlyDictionary_ValueGenericDictionary_DifferentElementType_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(ReadOnlyDictionary<string, int>), typeof(object));
            var value = new Dictionary<string, long> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_TargetNotSpecializedGenericDictionary_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(Dictionary<string, int>), typeof(object));
            var value = new OrderedDictionary<string, long> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericDictionaryOrEnumerableKeyValuePair_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(ReadOnlyDictionary<string, int>), typeof(object));
            List<int> value = [42];
            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericDictionary_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(ReadOnlyDictionary<string, int>), typeof(object));
            var value = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
