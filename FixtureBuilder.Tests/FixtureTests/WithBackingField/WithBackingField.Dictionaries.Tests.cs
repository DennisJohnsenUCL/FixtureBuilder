using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace FixtureBuilder.Tests.FixtureTests.WithBackingField
{
    internal class WithBackingFieldDictionariesTests
    {
        private readonly static string _text = "Test string";

        class DictionaryClass
        {
            private readonly Dictionary<int, string> _dictionary = null!;
            public List<KeyValuePair<int, string>> Dictionary => [.. _dictionary];
        }
        [Test]
        public void DictionaryField_SetsField()
        {
            var fieldName = "_dictionary";
            var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<DictionaryClass>().CreateUninitialized().WithBackingField(fieldName, c => c.Dictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.Dictionary.Single().Value, Is.EqualTo(_text));
        }

        class DictionaryObjectClass
        {
            private readonly Dictionary<object, object> _dictionary = null!;
            public List<KeyValuePair<int, string>> Dictionary => [.. _dictionary.Select(kvp => new KeyValuePair<int, string>((int)kvp.Key, (string)kvp.Value))];
        }
        [Test]
        public void DictionaryField_ObjectGenericParameters_SetsField()
        {
            var fieldName = "_dictionary";
            var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<DictionaryObjectClass>().CreateUninitialized().WithBackingField(fieldName, c => c.Dictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.Dictionary.Single().Value, Is.EqualTo(_text));
        }

        class DictionaryTypedClass
        {
            private readonly Dictionary<int, string> _dictionary = null!;
            public List<KeyValuePair<object, object>> Dictionary => [.. _dictionary.Select(kvp => new KeyValuePair<object, object>(kvp.Key, kvp.Value))];
        }
        [Test]
        public void DictionaryField_ObjectListTyping_SetsField()
        {
            var fieldName = "_dictionary";
            var dictionary = new List<KeyValuePair<object, object>>([new KeyValuePair<object, object>(1, _text)]);

            var fixture = Fixture.New<DictionaryTypedClass>().CreateUninitialized().WithBackingField(fieldName, c => c.Dictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.Dictionary.Single().Value, Is.EqualTo(_text));
        }

        class DictionarySortedListClass
        {
            private readonly Dictionary<int, string> _dictionary = null!;
            public SortedList Dictionary => new(_dictionary);
        }
        [Test]
        public void DictionaryField_SortedListProperty_SetsField()
        {
            var fieldName = "_dictionary";
            var dictionary = new SortedList() { { 1, _text } };

            var fixture = Fixture.New<DictionarySortedListClass>().CreateUninitialized().WithBackingField(fieldName, c => c.Dictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.Dictionary[1], Is.EqualTo(_text));
        }

        class IDictionaryClass
        {
            private readonly IDictionary<int, string> _iDictionary = null!;
            public List<KeyValuePair<int, string>> IDictionary => [.. _iDictionary];
        }
        [Test]
        public void IDictionaryField_SetsField()
        {
            var fieldName = "_iDictionary";
            var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<IDictionaryClass>().CreateUninitialized().WithBackingField(fieldName, c => c.IDictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.IDictionary.Single().Value, Is.EqualTo(_text));
        }

        class IDictionaryNonGenericClass
        {
            private readonly IDictionary _iDictionary = null!;
            public List<KeyValuePair<int, string>> IDictionary => [.. _iDictionary.Cast<DictionaryEntry>().Select(de => new KeyValuePair<int, string>((int)de.Key, (string)de.Value!))];
        }
        [Test]
        public void IDictionaryNonGenericField_SetsField()
        {
            var fieldName = "_iDictionary";
            var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<IDictionaryNonGenericClass>().CreateUninitialized().WithBackingField(fieldName, c => c.IDictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.IDictionary.Single().Value, Is.EqualTo(_text));
        }

        class IDictionaryNonGenericSortedListClass
        {
            private readonly IDictionary _iDictionary = null!;
            public SortedList IDictionary => new(_iDictionary);
        }
        [Test]
        public void IDictionaryNonGenericField_SortedListProperty_SetsField()
        {
            var fieldName = "_iDictionary";
            var dictionary = new SortedList { { 1, _text } };

            var fixture = Fixture.New<IDictionaryNonGenericSortedListClass>().CreateUninitialized().WithBackingField(fieldName, c => c.IDictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.IDictionary[1], Is.EqualTo(_text));
        }

        class ImmutableDictionaryClass
        {
            private readonly ImmutableDictionary<int, string> _immutableDictionary = null!;
            public List<KeyValuePair<int, string>> ImmutableDictionary => [.. _immutableDictionary];
        }
        [Test]
        public void ImmutableDictionaryField_SetsField()
        {
            var fieldName = "_immutableDictionary";
            var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<ImmutableDictionaryClass>().CreateUninitialized().WithBackingField(fieldName, c => c.ImmutableDictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.ImmutableDictionary.Single().Value, Is.EqualTo(_text));
        }

        class IImmutableDictionaryClass
        {
            private readonly IImmutableDictionary<int, string> _iImmutableDictionary = null!;
            public List<KeyValuePair<int, string>> IImmutableDictionary => [.. _iImmutableDictionary];
        }
        [Test]
        public void IImmutableDictionaryField_SetsField()
        {
            var fieldName = "_iImmutableDictionary";
            var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<IImmutableDictionaryClass>().CreateUninitialized().WithBackingField(fieldName, c => c.IImmutableDictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.IImmutableDictionary.Single().Value, Is.EqualTo(_text));
        }

        class SortedDictionaryClass
        {
            private readonly SortedDictionary<int, string> _sortedDictionary = null!;
            public List<KeyValuePair<int, string>> SortedDictionary => [.. _sortedDictionary];
        }
        [Test]
        public void SortedDictionaryField_SetsField()
        {
            var fieldName = "_sortedDictionary";
            var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<SortedDictionaryClass>().CreateUninitialized().WithBackingField(fieldName, c => c.SortedDictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.SortedDictionary.Single().Value, Is.EqualTo(_text));
        }

        class OrderedDictionaryClass
        {
            private readonly OrderedDictionary<int, string> _orderedDictionary = null!;
            public List<KeyValuePair<int, string>> OrderedDictionary => [.. _orderedDictionary];
        }
        [Test]
        public void OrderedDictionaryField_SetsField()
        {
            var fieldName = "_orderedDictionary";
            var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<OrderedDictionaryClass>().CreateUninitialized().WithBackingField(fieldName, c => c.OrderedDictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.OrderedDictionary.Single().Value, Is.EqualTo(_text));
        }

        class ImmutableSortedDictionaryClass
        {
            private readonly ImmutableSortedDictionary<int, string> _immutableSortedDictionary = null!;
            public List<KeyValuePair<int, string>> ImmutableSortedDictionary => [.. _immutableSortedDictionary];
        }
        [Test]
        public void ImmutableSortedDictionaryField_SetsField()
        {
            var fieldName = "_immutableSortedDictionary";
            var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<ImmutableSortedDictionaryClass>().CreateUninitialized().WithBackingField(fieldName, c => c.ImmutableSortedDictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.ImmutableSortedDictionary.Single().Value, Is.EqualTo(_text));
        }

        class FrozenDictionaryClass
        {
            private readonly FrozenDictionary<int, string> _frozenDictionary = null!;
            public List<KeyValuePair<int, string>> FrozenDictionary => [.. _frozenDictionary];
        }
        [Test]
        public void FrozenDictionaryField_SetsField()
        {
            var fieldName = "_frozenDictionary";
            var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<FrozenDictionaryClass>().CreateUninitialized().WithBackingField(fieldName, c => c.FrozenDictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.FrozenDictionary.Single().Value, Is.EqualTo(_text));
        }

        class ReadOnlyDictionaryClass
        {
            private readonly ReadOnlyDictionary<int, string> _readOnlyDictionary = null!;
            public List<KeyValuePair<int, string>> ReadOnlyDictionary => [.. _readOnlyDictionary];
        }
        [Test]
        public void ReadOnlyDictionaryField_SetsField()
        {
            var fieldName = "_readOnlyDictionary";
            var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<ReadOnlyDictionaryClass>().CreateUninitialized().WithBackingField(fieldName, c => c.ReadOnlyDictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.ReadOnlyDictionary.Single().Value, Is.EqualTo(_text));
        }

        class IReadOnlyDictionaryClass
        {
            private readonly IReadOnlyDictionary<int, string> _iReadOnlyDictionary = null!;
            public List<KeyValuePair<int, string>> IReadOnlyDictionary => [.. _iReadOnlyDictionary];
        }
        [Test]
        public void IReadOnlyDictionaryField_SetsField()
        {
            var fieldName = "_iReadOnlyDictionary";
            var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<IReadOnlyDictionaryClass>().CreateUninitialized().WithBackingField(fieldName, c => c.IReadOnlyDictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.IReadOnlyDictionary.Single().Value, Is.EqualTo(_text));
        }

        class SortedListClass
        {
            private readonly SortedList<int, string> _sortedList = null!;
            public List<KeyValuePair<int, string>> SortedList => [.. _sortedList];
        }
        [Test]
        public void SortedListField_SetsField()
        {
            var fieldName = "_sortedList";
            var sortedList = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<SortedListClass>().CreateUninitialized().WithBackingField(fieldName, c => c.SortedList, sortedList);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.SortedList.Single().Value, Is.EqualTo(_text));
        }

        class SortedListNonGenericClass
        {
            private readonly SortedList _sortedList = null!;
            public List<KeyValuePair<int, string>> SortedList => [.. _sortedList.Cast<DictionaryEntry>().Select(de => new KeyValuePair<int, string>((int)de.Key, (string)de.Value!))];
        }
        [Test]
        public void SortedListNonGenericField_SetsField()
        {
            var fieldName = "_sortedList";
            var sortedList = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<SortedListNonGenericClass>().CreateUninitialized().WithBackingField(fieldName, c => c.SortedList, sortedList);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.SortedList.Single().Value, Is.EqualTo(_text));
        }

        class SortedListNonGenericHashTableClass
        {
            private readonly SortedList _sortedList = null!;
            public Hashtable SortedList => new(_sortedList);
        }
        [Test]
        public void SortedListNonGenericField_HashTableProperty_SetsField()
        {
            var fieldName = "_sortedList";
            var sortedList = new Hashtable() { { 1, _text } };

            var fixture = Fixture.New<SortedListNonGenericHashTableClass>().CreateUninitialized().WithBackingField(fieldName, c => c.SortedList, sortedList);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.SortedList[1], Is.EqualTo(_text));
        }

        class HashtableClass
        {
            private readonly Hashtable _hashtable = null!;
            public List<KeyValuePair<int, string>> Hashtable => [.. _hashtable.Cast<DictionaryEntry>().Select(de => new KeyValuePair<int, string>((int)de.Key, (string)de.Value!))];
        }
        [Test]
        public void HashtableField_SetsField()
        {
            var fieldName = "_hashtable";
            var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<HashtableClass>().CreateUninitialized().WithBackingField(fieldName, c => c.Hashtable, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.Hashtable.Single().Value, Is.EqualTo(_text));
        }

        class HashtableSortedListClass
        {
            private readonly Hashtable _hashtable = null!;
            public SortedList Hashtable => new(_hashtable);
        }
        [Test]
        public void HashtableField_SortedListProperty_SetsField()
        {
            var fieldName = "_hashtable";
            var dictionary = new SortedList() { { 1, _text } };

            var fixture = Fixture.New<HashtableSortedListClass>().CreateUninitialized().WithBackingField(fieldName, c => c.Hashtable, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.Hashtable[1], Is.EqualTo(_text));
        }

        class ConcurrentDictionaryClass
        {
            private readonly ConcurrentDictionary<int, string> _concurrentDictionary = null!;
            public List<KeyValuePair<int, string>> ConcurrentDictionary => [.. _concurrentDictionary];
        }
        [Test]
        public void ConcurrentDictionaryField_SetsField()
        {
            var fieldName = "_concurrentDictionary";
            var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

            var fixture = Fixture.New<ConcurrentDictionaryClass>().CreateUninitialized().WithBackingField(fieldName, c => c.ConcurrentDictionary, dictionary);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.ConcurrentDictionary.Single().Value, Is.EqualTo(_text));
        }
    }
}
