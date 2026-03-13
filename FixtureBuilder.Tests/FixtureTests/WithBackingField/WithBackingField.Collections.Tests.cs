using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;

#pragma warning disable CS0649

namespace FixtureBuilder.Tests.FixtureTests.WithBackingField
{
    internal sealed partial class WithBackingFieldCollectionsTests
    {
        private readonly static string _text = "Test string";
        private readonly static int _number = 123;

        class ListClass
        {
            private readonly List<string> _list = null!;
            public IEnumerable<string> List => _list;
        }
        [Test]
        public void ListField_SetsField()
        {
            var fieldName = "_list";

            var fixture = Fixture.New<ListClass>().CreateUninitialized().WithBackingField(fieldName, c => c.List, [_text]);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.List.Single(), Is.EqualTo(_text));
        }

        class ImmutableListClass
        {
            private readonly ImmutableList<string> _immutableList = null!;
            public IEnumerable<string> ImmutableList => _immutableList;
        }
        [Test]
        public void ImmutableListField_SetsField()
        {
            var fieldName = "_immutableList";

            var fixture = Fixture.New<ImmutableListClass>().CreateUninitialized().WithBackingField(fieldName, c => c.ImmutableList, [_text]);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.ImmutableList.Single(), Is.EqualTo(_text));
        }

        class FrozenSetClass
        {
            private readonly FrozenSet<string> _frozenSet = null!;
            public IEnumerable<string> FrozenSet => _frozenSet;
        }
        [Test]
        public void FrozenSetField_SetsField()
        {
            var fieldName = "_frozenSet";

            var fixture = Fixture.New<FrozenSetClass>().CreateUninitialized().WithBackingField(fieldName, c => c.FrozenSet, [_text]);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.FrozenSet.Single(), Is.EqualTo(_text));
        }

        class ArrayClass
        {
            private readonly string[] _stringArray = null!;
            public IEnumerable<string> StringArray => _stringArray;
        }
        [Test]
        public void ArrayField_SetsField()
        {
            var fieldName = "_stringArray";

            var fixture = Fixture.New<ArrayClass>().CreateUninitialized().WithBackingField(fieldName, c => c.StringArray, [_text]);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.StringArray.Single(), Is.EqualTo(_text));
        }

        class ArrayListClass
        {
            private readonly ArrayList _arrayList = null!;
            public IEnumerable<object> ArrayList => _arrayList.Cast<object>();
        }
        [Test]
        public void ArrayListField_SetsField()
        {
            var fieldName = "_arrayList";

            var fixture = Fixture.New<ArrayListClass>().CreateUninitialized().WithBackingField(fieldName, c => c.ArrayList, [_number]);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.ArrayList.Single(), Is.EqualTo(_number));
        }

        class ListArrayClass
        {
            private readonly List<string> _list = null!;
            public string[] List => [.. _list];
        }
        [Test]
        public void ListField_ArrayProperty_SetsField()
        {
            var fieldName = "_list";

            var fixture = Fixture.New<ListArrayClass>().CreateUninitialized().WithBackingField(fieldName, c => c.List, [_text]);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.List.Single(), Is.EqualTo(_text));
        }

        class IListNonGenericStackClass
        {
            private readonly IList _iList = null!;
            public Stack IList => new(_iList);
        }
        [Test]
        public void IListNonGenericField_StackProperty_SetsField()
        {
            var fieldName = "_iList";

            var fixture = Fixture.New<IListNonGenericStackClass>().CreateUninitialized().WithBackingField(fieldName, c => c.IList, new Stack(new List<string>([_text])));
            var field = TestHelper.GetFixture(fixture);
            Assert.That(field.IList.Peek(), Is.EqualTo(_text));
        }
        class BlockingCollectionClass
        {
            private readonly BlockingCollection<string> _collection = null!;
            public IEnumerable<string> Collection => _collection;
        }
        [Test]
        public void BlockingCollectionField_SetsField()
        {
            var fieldName = "_collection";

            var fixture = Fixture.New<BlockingCollectionClass>().CreateUninitialized().WithBackingField(fieldName, c => c.Collection, [_text]);
            var field = TestHelper.GetFixture(fixture);

            Assert.That(field.Collection.Single(), Is.EqualTo(_text));
        }
    }
}
