using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace FixtureBuilder.Tests.FixtureTests.WithBackingField
{
    internal sealed class WithBackingFieldDictionariesTests
    {
        private class PairsOverFieldClass<TField> where TField : IEnumerable<KeyValuePair<int, string>>
        {
            private readonly TField _pairs = default!;
            public List<KeyValuePair<int, string>> Pairs => [.. _pairs];
        }

        [Test]
        public void DictionaryField_SetsConvertedDictionary()
        {
            var fixture = TestHelper.MakeFixture<PairsOverFieldClass<Dictionary<int, string>>>();
            List<KeyValuePair<int, string>> pairs = [new(1, "value")];

            var result = fixture.WithBackingField(x => x.Pairs, pairs, "_pairs").Build();

            Assert.That(result.Pairs, Is.EqualTo(pairs));
        }

        [Test]
        public void ImmutableDictionaryField_SetsConvertedDictionary()
        {
            var fixture = TestHelper.MakeFixture<PairsOverFieldClass<ImmutableDictionary<int, string>>>();
            List<KeyValuePair<int, string>> pairs = [new(1, "value")];

            var result = fixture.WithBackingField(x => x.Pairs, pairs, "_pairs").Build();

            Assert.That(result.Pairs, Is.EqualTo(pairs));
        }

        [Test]
        public void FrozenDictionaryField_SetsConvertedDictionary()
        {
            var fixture = TestHelper.MakeFixture<PairsOverFieldClass<FrozenDictionary<int, string>>>();
            List<KeyValuePair<int, string>> pairs = [new(1, "value")];

            var result = fixture.WithBackingField(x => x.Pairs, pairs, "_pairs").Build();

            Assert.That(result.Pairs, Is.EqualTo(pairs));
        }

        private class ObjectDictionaryFieldClass
        {
            private readonly Dictionary<object, object> _pairs = null!;
            public List<KeyValuePair<int, string>> Pairs => [.. _pairs.Select(pair => new KeyValuePair<int, string>((int)pair.Key, (string)pair.Value))];
        }

        [Test]
        public void ObjectDictionaryField_SetsConvertedDictionary()
        {
            var fixture = TestHelper.MakeFixture<ObjectDictionaryFieldClass>();
            List<KeyValuePair<int, string>> pairs = [new(1, "value")];

            var result = fixture.WithBackingField(x => x.Pairs, pairs, "_pairs").Build();

            Assert.That(result.Pairs, Is.EqualTo(pairs));
        }

        [Test]
        public void ObjectDictionaryField_EmptySource_SetsEmptyDictionary()
        {
            var fixture = TestHelper.MakeFixture<ObjectDictionaryFieldClass>();
            List<KeyValuePair<int, string>> pairs = [];

            var result = fixture.WithBackingField(x => x.Pairs, pairs, "_pairs").Build();

            Assert.That(result.Pairs, Is.Empty);
        }

        private class NonGenericDictionaryFieldClass
        {
            private readonly IDictionary _pairs = null!;
            public List<KeyValuePair<int, string>> Pairs => [.. _pairs.Cast<DictionaryEntry>().Select(entry => new KeyValuePair<int, string>((int)entry.Key, (string)entry.Value!))];
        }

        [Test]
        public void NonGenericDictionaryField_SetsConvertedDictionary()
        {
            var fixture = TestHelper.MakeFixture<NonGenericDictionaryFieldClass>();
            List<KeyValuePair<int, string>> pairs = [new(1, "value")];

            var result = fixture.WithBackingField(x => x.Pairs, pairs, "_pairs").Build();

            Assert.That(result.Pairs, Is.EqualTo(pairs));
        }

        private class SortedListOverDictionaryFieldClass
        {
            private readonly Dictionary<int, string> _pairs = null!;
            public SortedList Pairs => new(_pairs);
        }

        [Test]
        public void DictionaryField_SortedListProperty_SetsConvertedDictionary()
        {
            var fixture = TestHelper.MakeFixture<SortedListOverDictionaryFieldClass>();
            var pairs = new SortedList { [1] = "value" };

            var result = fixture.WithBackingField(x => x.Pairs, pairs, "_pairs").Build();

            Assert.That(result.Pairs[1], Is.EqualTo("value"));
        }
    }
}
