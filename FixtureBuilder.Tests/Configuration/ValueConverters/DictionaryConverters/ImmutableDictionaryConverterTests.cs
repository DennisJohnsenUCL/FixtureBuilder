using System.Collections.Immutable;
using System.Collections.ObjectModel;
using FixtureBuilder.Configuration.ValueConverters.DictionaryConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.DictionaryConverters
{
    internal sealed class ImmutableDictionaryConverterTests
    {
        [Test]
        public void Convert_TargetImmutableDictionary_ValueGenericDictionary_SameElementType_Converts()
        {
            var request = new TypeRequest(typeof(ImmutableDictionary<string, int>), typeof(object));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = ImmutableDictionary.CreateRange(value);
            var converter = new ImmutableDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableDictionary_ValueGenericEnumerableKeyValuePair_SameElementType_Converts()
        {
            var request = new TypeRequest(typeof(ImmutableDictionary<string, int>), typeof(object));
            List<KeyValuePair<string, int>> value = [new("one", 1), new("two", 2), new("three", 3)];
            var expected = ImmutableDictionary.CreateRange(value);
            var converter = new ImmutableDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetImmutableSortedDictionary_ValueGenericDictionary_SameElementType_Converts()
        {
            var request = new TypeRequest(typeof(ImmutableSortedDictionary<string, int>), typeof(object));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = ImmutableSortedDictionary.CreateRange(value);
            var converter = new ImmutableDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetNotImmutableDictionary_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(ReadOnlyDictionary<string, int>), typeof(object));
            var value = new Dictionary<string, long> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new ImmutableDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericDictionaryOrEnumerableKeyValuePair_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(ImmutableDictionary<string, int>), typeof(object));
            List<int> value = [42];
            var converter = new ImmutableDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
