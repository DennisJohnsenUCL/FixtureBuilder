using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace FixtureBuilder.Tests
{
    internal sealed partial class WithFieldTests
    {
        class StringListClass
        {
            private readonly List<string> _stringList = null!;
            public List<string> StringList => _stringList;
        }
        [Test]
        public void CollectionTypeField_StringParameter_ThrowsException()
        {
            var fieldName = "_stringList";

            Assert.Throws<InvalidOperationException>(() => Fixture.New<StringListClass>().BypassConstructor().WithField(fieldName, _text));
        }

        [Test]
        public void CollectionTypeField_CollectionParameter_SetsField()
        {
            var fieldName = "_stringList";

            var fixture = Fixture.New<StringListClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.StringList[0], Is.EqualTo(_text));
        }

        [Test]
        public void CollectionTypeField_CollectionParameters_SetsField()
        {
            var fieldName = "_stringList";
            var secondEntry = "More test";

            var fixture = Fixture.New<StringListClass>().BypassConstructor().WithField(fieldName, [_text, secondEntry]);
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.StringList[0], Is.EqualTo(_text));
                Assert.That(field.StringList[1], Is.EqualTo(secondEntry));
            }
        }

        class HashSetClass
        {
            private readonly HashSet<string> _stringHashSet = null!;
            public HashSet<string> StringHashSet => _stringHashSet;
        }
        [Test]
        public void HashSetField_SetsField()
        {
            var fieldName = "_stringHashSet";

            var fixture = Fixture.New<HashSetClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.StringHashSet.Single(), Is.EqualTo(_text));
        }

        class ArrayClass
        {
            private readonly string[] _stringArray = null!;
            public string[] StringArray => _stringArray;
        }
        [Test]
        public void ArrayField_SetsField()
        {
            var fieldName = "_stringArray";

            var fixture = Fixture.New<ArrayClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.StringArray[0], Is.EqualTo(_text));
        }

        class ArrayListClass
        {
            private readonly ArrayList _arrayList = null!;
            public ArrayList ArrayList => _arrayList;
        }
        [Test]
        public void ArrayListField_SetsField()
        {
            var fieldName = "_arrayList";

            var fixture = Fixture.New<ArrayListClass>().BypassConstructor().WithField(fieldName, [_number]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ArrayList[0], Is.EqualTo(_number));
        }

        class IReadOnlyListClass
        {
            private readonly IReadOnlyList<string> _iReadOnlyList = null!;
            public IReadOnlyList<string> IReadOnlyList => _iReadOnlyList;
        }
        [Test]
        public void IReadOnlyListField_SetsField()
        {
            var fieldName = "_iReadOnlyList";

            var fixture = Fixture.New<IReadOnlyListClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IReadOnlyList[0], Is.EqualTo(_text));
        }

        class ImmutableListClass
        {
            private readonly ImmutableList<string> _immutableList = null!;
            public ImmutableList<string> ImmutableList => _immutableList;
        }
        [Test]
        public void ImmutableListField_SetsField()
        {
            var fieldName = "_immutableList";

            var fixture = Fixture.New<ImmutableListClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ImmutableList[0], Is.EqualTo(_text));
        }

        class ReadOnlyCollectionClass
        {
            private readonly ReadOnlyCollection<string> _readOnlyCollection = null!;
            public ReadOnlyCollection<string> ReadOnlyCollection => _readOnlyCollection;
        }
        [Test]
        public void ReadOnlyCollectionField_SetsField()
        {
            var fieldName = "_readOnlyCollection";

            var fixture = Fixture.New<ReadOnlyCollectionClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ReadOnlyCollection[0], Is.EqualTo(_text));
        }

        class IImmutableListClass
        {
            private readonly IImmutableList<string> _iImmutableList = null!;
            public IImmutableList<string> IImmutableList => _iImmutableList;
        }
        [Test]
        public void IImmutableListField_SetsField()
        {
            var fieldName = "_iImmutableList";

            var fixture = Fixture.New<IImmutableListClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IImmutableList[0], Is.EqualTo(_text));
        }

        class DictionaryClass
        {
            private readonly Dictionary<int, string> _dictionary = null!;
            public Dictionary<int, string> Dictionary => _dictionary;
        }
        [Test]
        public void DictionaryField_SetsField()
        {
            var fieldName = "_dictionary";
            var dictionary = new Dictionary<int, string>() { { 1, "test" } };

            var fixture = Fixture.New<DictionaryClass>().BypassConstructor().WithField(fieldName, dictionary);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Dictionary[1], Is.EqualTo("test"));
        }

        class IDictionaryClass
        {
            private readonly IDictionary _iDictionary = null!;
            public IDictionary IDictionary => _iDictionary;
        }
        [Test]
        public void IDictionaryField_SetsField()
        {
            var fieldName = "_iDictionary";
            var dictionary = new Dictionary<int, string>() { { 1, "test" } };

            var fixture = Fixture.New<IDictionaryClass>().BypassConstructor().WithField(fieldName, dictionary);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IDictionary[1], Is.EqualTo("test"));
        }

        class IListClass
        {
            private readonly IList<string> _iList = null!;
            public IList<string> IList => _iList;
        }
        [Test]
        public void IListField_SetsField()
        {
            var fieldName = "_iList";

            var fixture = Fixture.New<IListClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IList[0], Is.EqualTo(_text));
        }

        class IEnumerableClass
        {
            private readonly IEnumerable<string> _iEnumerable = null!;
            public IEnumerable<string> IEnumerable => _iEnumerable;
        }
        [Test]
        public void IEnumerableField_SetsField()
        {
            var fieldName = "_iEnumerable";

            var fixture = Fixture.New<IEnumerableClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IEnumerable.First(), Is.EqualTo(_text));
        }

        class IReadOnlyCollectionClass
        {
            private readonly IReadOnlyCollection<string> _iReadOnlyCollection = null!;
            public IReadOnlyCollection<string> IReadOnlyCollection => _iReadOnlyCollection;
        }
        [Test]
        public void IReadOnlyCollectionField_SetsField()
        {
            var fieldName = "_iReadOnlyCollection";

            var fixture = Fixture.New<IReadOnlyCollectionClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IReadOnlyCollection.First(), Is.EqualTo(_text));
        }

        class ICollectionClass
        {
            private readonly ICollection<string> _iCollection = null!;
            public ICollection<string> ICollection => _iCollection;
        }
        [Test]
        public void ICollectionField_SetsField()
        {
            var fieldName = "_iCollection";

            var fixture = Fixture.New<ICollectionClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ICollection.First(), Is.EqualTo(_text));
        }

        class ISetClass
        {
            private readonly ISet<string> _iSet = null!;
            public ISet<string> ISet => _iSet;
        }
        [Test]
        public void ISetField_SetsField()
        {
            var fieldName = "_iSet";

            var fixture = Fixture.New<ISetClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ISet.First(), Is.EqualTo(_text));
        }

        class IReadOnlySetClass
        {
            private readonly IReadOnlySet<string> _iReadOnlySet = null!;
            public IReadOnlySet<string> IReadOnlySet => _iReadOnlySet;
        }
        [Test]
        public void IReadOnlySetField_SetsField()
        {
            var fieldName = "_iReadOnlySet";

            var fixture = Fixture.New<IReadOnlySetClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IReadOnlySet.First(), Is.EqualTo(_text));
        }

        class IImmutableSetClass
        {
            private readonly IImmutableSet<string> _iImmutableSet = null!;
            public IImmutableSet<string> IImmutableSet => _iImmutableSet;
        }
        [Test]
        public void IImmutableSetField_SetsField()
        {
            var fieldName = "_iImmutableSet";

            var fixture = Fixture.New<IImmutableSetClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IImmutableSet.First(), Is.EqualTo(_text));
        }

        class ListClass
        {
            private readonly List<string> _list = null!;
            public List<string> List => _list;
        }
        [Test]
        public void ListField_SetsField()
        {
            var fieldName = "_list";

            var fixture = Fixture.New<ListClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.List[0], Is.EqualTo(_text));
        }

        class ImmutableArrayClass
        {
            private readonly ImmutableArray<string> _immutableArray;
            public ImmutableArray<string> ImmutableArray => _immutableArray;
        }
        [Test]
        public void ImmutableArrayField_SetsField()
        {
            var fieldName = "_immutableArray";

            var fixture = Fixture.New<ImmutableArrayClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ImmutableArray[0], Is.EqualTo(_text));
        }

        class ImmutableHashSetClass
        {
            private readonly ImmutableHashSet<string> _immutableHashSet = null!;
            public ImmutableHashSet<string> ImmutableHashSet => _immutableHashSet;
        }
        [Test]
        public void ImmutableHashSetField_SetsField()
        {
            var fieldName = "_immutableHashSet";

            var fixture = Fixture.New<ImmutableHashSetClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ImmutableHashSet.First(), Is.EqualTo(_text));
        }

        class IListNonGenericClass
        {
            private readonly IList _iList = null!;
            public IList IList => _iList;
        }
        [Test]
        public void IListNonGenericField_SetsField()
        {
            var fieldName = "_iList";

            var fixture = Fixture.New<IListNonGenericClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IList[0], Is.EqualTo(_text));
        }

        class ICollectionNonGenericClass
        {
            private readonly ICollection _iCollection = null!;
            public ICollection ICollection => _iCollection;
        }
        [Test]
        public void ICollectionNonGenericField_SetsField()
        {
            var fieldName = "_iCollection";

            var fixture = Fixture.New<ICollectionNonGenericClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ICollection.Cast<string>().First(), Is.EqualTo(_text));
        }

        class IEnumerableNonGenericClass
        {
            private readonly IEnumerable _iEnumerable = null!;
            public IEnumerable IEnumerable => _iEnumerable;
        }
        [Test]
        public void IEnumerableNonGenericField_SetsField()
        {
            var fieldName = "_iEnumerable";

            var fixture = Fixture.New<IEnumerableNonGenericClass>().BypassConstructor().WithField(fieldName, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IEnumerable.Cast<string>().First(), Is.EqualTo(_text));
        }
    }
}
