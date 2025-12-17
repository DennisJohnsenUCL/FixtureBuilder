using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace FixtureBuilder.Tests
{
    internal sealed partial class WithFieldTests
    {
        private readonly static string _text = "Test string";
        private readonly static int _number = 123;

        [Test]
        public void ClassPrivateField_SetsField()
        {
            var fieldName = "_privateExplicitField";

            var fixture = Fixture.New<TestClass>().BypassConstructor().WithField(fieldName, _text).Build();

            Assert.That(fixture.PrivateExplicitField, Is.EqualTo(_text));
        }

        [Test]
        public void IncorrectFieldName_ThrowsException()
        {
            var fieldName = "_notAField";

            Assert.Throws<InvalidOperationException>(() => Fixture.New<TestClass>().BypassConstructor().WithField(fieldName, _text).Build());
        }

        [Test]
        public void IncorrectFieldType_ThrowsException()
        {
            var fieldName = "_privateExplicitField";

            Assert.Throws<InvalidOperationException>(() => Fixture.New<TestClass>().BypassConstructor().WithField(fieldName, _number).Build());
        }

        [Test]
        public void InheritedProtectedField_SetsField()
        {
            var fieldName = "_inheritedField";

            var derivedTestClass = Fixture.New<DerivedTestClass>().BypassConstructor().WithField(fieldName, _text).Build();

            Assert.That(derivedTestClass.InheritedFieldGetter, Is.EqualTo(_text));
        }

        [Test]
        public void FieldNameGiven_SkipConstructionMethods_ConstructsFixture()
        {
            var fieldName = "_privateExplicitField";

            var fixture = Fixture.New<TestClass>().WithField(fieldName, _text).Build();

            Assert.That(fixture.PrivateExplicitField, Is.EqualTo(_text));
        }

        [Test]
        public void CollectionTypeField_StringParameter_ThrowsException()
        {
            var fieldName = "_stringListField";

            Assert.Throws<InvalidOperationException>(() => Fixture.New<TestClass>().BypassConstructor().WithField(fieldName, _text).Build());
        }

        [Test]
        public void CollectionTypeField_CollectionParameter_SetsField()
        {
            var fieldName = "_stringListField";

            var fixture = Fixture.New<TestClass>().BypassConstructor().WithField(fieldName, [_text]).Build();

            Assert.That(fixture.StringListProp[0], Is.EqualTo(_text));
        }

        [Test]
        public void CollectionTypeField_CollectionParameters_SetsField()
        {
            var fieldName = "_stringListField";
            var secondEntry = "More test";

            var fixture = Fixture.New<TestClass>().BypassConstructor().WithField(fieldName, [_text, secondEntry]).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(fixture.StringListProp[0], Is.EqualTo(_text));
                Assert.That(fixture.StringListProp[1], Is.EqualTo(secondEntry));
            }
        }

        [Test]
        public void HashSetField_SetsField()
        {
            var fieldName = "_hashSet";

            var fixture = Fixture.New<TestClass>().BypassConstructor().WithField(fieldName, [_number]).Build();

            Assert.That(fixture.HashSet.Single(), Is.EqualTo(_number));
        }

        [Test]
        public void ArrayField_SetsField()
        {
            var fieldName = "_array";

            var fixture = Fixture.New<TestClass>().BypassConstructor().WithField(fieldName, [_number]).Build();

            Assert.That(fixture.Array[0], Is.EqualTo(_number));
        }

        [Test]
        public void ArrayListField_SetsField()
        {
            var fieldName = "_arrayList";

            var fixture = Fixture.New<TestClass>().BypassConstructor().WithField(fieldName, [_number]).Build();

            Assert.That(fixture.ArrayList[0], Is.EqualTo(_number));
        }

        [Test]
        public void IReadOnlyListField_SetsField()
        {
            var fieldName = "_readOnlyList";

            var fixture = Fixture.New<TestClass>().BypassConstructor().WithField(fieldName, [_number]).Build();

            Assert.That(fixture.IReadOnlyList[0], Is.EqualTo(_number));
        }

        [Test]
        public void ImmutableListField_SetsField()
        {
            var fieldName = "_immutableList";

            var fixture = Fixture.New<TestClass>().BypassConstructor().WithField(fieldName, [_number]).Build();

            Assert.That(fixture.ImmutableList[0], Is.EqualTo(_number));
        }

        class ReadOnlyCollectionClass
        {
            private readonly ReadOnlyCollection<int> _readOnlyCollection = null!;
            public ReadOnlyCollection<int> ReadOnlyCollection => _readOnlyCollection;
        }
        [Test]
        public void ReadOnlyCollectionField_SetsField()
        {
            var fieldName = "_readOnlyCollection";

            var fixture = Fixture.New<ReadOnlyCollectionClass>().BypassConstructor().WithField(fieldName, [_number]).Build();

            Assert.That(fixture.ReadOnlyCollection[0], Is.EqualTo(_number));
        }

        class IImmutableListClass
        {
            private readonly IImmutableList<int> _iImmutableList = null!;
            public IImmutableList<int> IImmutableList => _iImmutableList;
        }
        [Test]
        public void IImmutableListField_SetsField()
        {
            var fieldName = "_iImmutableList";

            var fixture = Fixture.New<IImmutableListClass>().BypassConstructor().WithField(fieldName, [_number]).Build();

            Assert.That(fixture.IImmutableList[0], Is.EqualTo(_number));
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

            var fixture = Fixture.New<DictionaryClass>().BypassConstructor().WithField(fieldName, dictionary).Build();

            Assert.That(fixture.Dictionary[1], Is.EqualTo("test"));
        }
    }
}
