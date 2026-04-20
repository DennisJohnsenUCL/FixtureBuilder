using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;

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
            var fixture = TestHelper.MakeFixture<DictionaryClass>();

            fixture.WithBackingField(c => c.Dictionary, dictionary, fieldName);

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
            var fixture = TestHelper.MakeFixture<DictionaryObjectClass>();

            fixture.WithBackingField(c => c.Dictionary, dictionary, fieldName);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Dictionary.Single().Value, Is.EqualTo(_text));
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
            var fixture = TestHelper.MakeFixture<ImmutableDictionaryClass>();

            fixture.WithBackingField(c => c.ImmutableDictionary, dictionary, fieldName);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.ImmutableDictionary.Single().Value, Is.EqualTo(_text));
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
            var fixture = TestHelper.MakeFixture<FrozenDictionaryClass>();

            fixture.WithBackingField(c => c.FrozenDictionary, dictionary, fieldName);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.FrozenDictionary.Single().Value, Is.EqualTo(_text));
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
            var fixture = TestHelper.MakeFixture<IDictionaryNonGenericClass>();

            fixture.WithBackingField(c => c.IDictionary, dictionary, fieldName);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.IDictionary.Single().Value, Is.EqualTo(_text));
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
            var fixture = TestHelper.MakeFixture<DictionarySortedListClass>();

            fixture.WithBackingField(c => c.Dictionary, dictionary, fieldName);

            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.Dictionary[1], Is.EqualTo(_text));
        }
    }
}
