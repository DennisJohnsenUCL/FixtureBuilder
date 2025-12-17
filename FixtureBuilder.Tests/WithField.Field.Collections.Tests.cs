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
    }
}
