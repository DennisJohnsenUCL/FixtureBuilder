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
        public void Convert_TargetFrozenDictionary_ValueGenericDictionary_SameElementType_Converts()
        {
            var request = new TypeRequest(typeof(FrozenDictionary<string, int>), typeof(object));
            var value = new Dictionary<string, int> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var expected = value.ToFrozenDictionary(null);
            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetFrozenDictionary_ValueGenericEnumerableKeyValuePair_SameElementType_Converts()
        {
            var request = new TypeRequest(typeof(FrozenDictionary<string, int>), typeof(object));
            List<KeyValuePair<string, int>> value = [new("one", 1), new("two", 2), new("three", 3)];
            var expected = value.ToFrozenDictionary(null);
            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_TargetNotFrozenDictionary_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(ReadOnlyDictionary<string, int>), typeof(object));
            var value = new Dictionary<string, long> { { "one", 1 }, { "two", 2 }, { "three", 3 } };
            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_ValueNotGenericDictionaryOrEnumerableKeyValuePair_ReturnsNoResult()
        {
            var request = new TypeRequest(typeof(FrozenDictionary<string, int>), typeof(object));
            List<int> value = [42];
            var converter = new FrozenDictionaryConverter();

            var result = converter.Convert(request, value, Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
