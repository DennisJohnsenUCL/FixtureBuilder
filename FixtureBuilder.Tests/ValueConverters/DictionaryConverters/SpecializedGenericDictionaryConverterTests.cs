using FixtureBuilder.FixtureContexts;
using FixtureBuilder.ValueConverters.DictionaryConverters;
using Moq;
using System.Collections;
using System.Collections.ObjectModel;

namespace FixtureBuilder.Tests.ValueConverters.DictionaryConverters
{
    internal sealed class SpecializedGenericDictionaryConverterTests
    {
        [Test]
        public void Constructor_Constructs()
        {
            Assert.DoesNotThrow(() => new SpecializedGenericDictionaryConverter());
        }

        [Test]
        public void Convert_TargetReadOnlyDictionary_ValueGenericDictionary_SameElementType_Converts()
        {
            var target = typeof(ReadOnlyDictionary<string, int>);
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new ReadOnlyDictionary<string, int>(value);
            var context = new Mock<IFixtureContext>().Object;

            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetReadOnlyDictionary_ValueGenericEnumerableKeyValuePair_SameElementType_Converts()
        {
            var target = typeof(ReadOnlyDictionary<string, int>);
            var value = new List<KeyValuePair<string, int>> { new("one", 1), new("two", 2), new("three", 3) };
            var expected = new ReadOnlyDictionary<string, int>(new Dictionary<string, int>(value));
            var context = new Mock<IFixtureContext>().Object;

            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetSortedDictionary_ValueGenericDictionary_SameElementType_Converts()
        {
            var target = typeof(SortedDictionary<string, int>);
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new SortedDictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetSortedList_ValueGenericDictionary_SameElementType_Converts()
        {
            var target = typeof(SortedList<string, int>);
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = new SortedList<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetReadOnlyDictionary_ValueGenericDictionary_DifferentElementType_ReturnsNull()
        {
            var target = typeof(ReadOnlyDictionary<string, int>);
            var value = new Dictionary<string, long> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_TargetNotSpecializedGenericDictionary_ReturnsNull()
        {
            var target = typeof(Dictionary<string, int>);
            var value = new OrderedDictionary<string, long> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_ValueNotGenericDictionaryOrEnumerableKeyValuePair_ReturnsNull()
        {
            var target = typeof(ReadOnlyDictionary<string, int>);
            var value = new List<int>() { 42 };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_ValueNotGenericDictionary_ReturnsNull()
        {
            var target = typeof(ReadOnlyDictionary<string, int>);
            var value = new Hashtable { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var context = new Mock<IFixtureContext>().Object;

            var converter = new SpecializedGenericDictionaryConverter();

            var result = converter.Convert(target, value, context);

            Assert.That(result, Is.Null);
        }
    }
}
