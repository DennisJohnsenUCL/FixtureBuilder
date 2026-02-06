using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

#pragma warning disable CS0649

namespace FixtureBuilder.Tests.WithBackingField
{
    internal sealed partial class WithBackingFieldCollectionsTests
    {
        private readonly static string _text = "Test string";
        private readonly static int _number = 123;

        #region Lists
        class ListClass
        {
            private readonly List<string> _list = null!;
            public IEnumerable<string> List => _list;
        }
        [Test]
        public void ListField_SetsField()
        {
            var fieldName = "_list";

            var fixture = Fixture.New<ListClass>().BypassConstructor().WithBackingField(fieldName, c => c.List, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.List.Single(), Is.EqualTo(_text));
        }

        class ListArrayListClass
        {
            private readonly List<string> _list = null!;
            public ArrayList List => new(_list);
        }
        [Test]
        public void ListField_ArrayListProperty_SetsField()
        {
            var fieldName = "_list";

            var fixture = Fixture.New<ListClass>().BypassConstructor().WithBackingField(fieldName, c => c.List, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.List.Single(), Is.EqualTo(_text));
        }

        class IReadOnlyListClass
        {
            private readonly IReadOnlyList<string> _iReadOnlyList = null!;
            public HashSet<string> IReadOnlyList => [.. _iReadOnlyList];
        }
        [Test]
        public void IReadOnlyListField_SetsField()
        {
            var fieldName = "_iReadOnlyList";

            var fixture = Fixture.New<IReadOnlyListClass>().BypassConstructor().WithBackingField(fieldName, c => c.IReadOnlyList, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IReadOnlyList.Single(), Is.EqualTo(_text));
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

            var fixture = Fixture.New<ImmutableListClass>().BypassConstructor().WithBackingField(fieldName, c => c.ImmutableList, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ImmutableList.Single(), Is.EqualTo(_text));
        }

        class IImmutableListClass
        {
            private readonly IImmutableList<string> _iImmutableList = null!;
            public IEnumerable<string> IImmutableList => _iImmutableList;
        }
        [Test]
        public void IImmutableListField_SetsField()
        {
            var fieldName = "_iImmutableList";

            var fixture = Fixture.New<IImmutableListClass>().BypassConstructor().WithBackingField(fieldName, c => c.IImmutableList, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IImmutableList.Single(), Is.EqualTo(_text));
        }

        class IListClass
        {
            private readonly IList<string> _iList = null!;
            public HashSet<string> IList => [.. _iList];
        }
        [Test]
        public void IListField_SetsField()
        {
            var fieldName = "_iList";

            var fixture = Fixture.New<IListClass>().BypassConstructor().WithBackingField(fieldName, c => c.IList, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IList.Single(), Is.EqualTo(_text));
        }

        class IListNonGenericClass
        {
            private readonly IList _iList = null!;
            public HashSet<string> IList => [.. _iList.Cast<string>()];
        }
        [Test]
        public void IListNonGenericField_SetsField()
        {
            var fieldName = "_iList";

            var fixture = Fixture.New<IListNonGenericClass>().BypassConstructor().WithBackingField(fieldName, c => c.IList, [_text]);
            var field = Helpers.GetFixture(fixture);
            Assert.That(field.IList.Single(), Is.EqualTo(_text));
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

            var fixture = Fixture.New<IListNonGenericStackClass>().BypassConstructor().WithBackingField(fieldName, c => c.IList, new Stack(new List<string>([_text])));
            var field = Helpers.GetFixture(fixture);
            Assert.That(field.IList.Peek(), Is.EqualTo(_text));
        }

        class ConcurrentBagClass
        {
            private readonly ConcurrentBag<string> _concurrentBag = null!;
            public IEnumerable<string> ConcurrentBag => _concurrentBag;
        }
        [Test]
        public void ConcurrentBagField_SetsField()
        {
            var fieldName = "_concurrentBag";

            var fixture = Fixture.New<ConcurrentBagClass>().BypassConstructor().WithBackingField(fieldName, c => c.ConcurrentBag, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ConcurrentBag.Single(), Is.EqualTo(_text));
        }
        #endregion

        #region Arrays
        class ArrayClass
        {
            private readonly string[] _stringArray = null!;
            public IEnumerable<string> StringArray => _stringArray;
        }
        [Test]
        public void ArrayField_SetsField()
        {
            var fieldName = "_stringArray";

            var fixture = Fixture.New<ArrayClass>().BypassConstructor().WithBackingField(fieldName, c => c.StringArray, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.StringArray.Single(), Is.EqualTo(_text));
        }

        class ImmutableArrayClass
        {
            private readonly ImmutableArray<string> _immutableArray;
            public IEnumerable<string> ImmutableArray => _immutableArray;
        }
        [Test]
        public void ImmutableArrayField_SetsField()
        {
            var fieldName = "_immutableArray";

            var fixture = Fixture.New<ImmutableArrayClass>().BypassConstructor().WithBackingField(fieldName, c => c.ImmutableArray, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ImmutableArray.Single(), Is.EqualTo(_text));
        }
        #endregion

        #region Sets
        class HashSetClass
        {
            private readonly HashSet<string> _hashSet = null!;
            public IEnumerable<string> HashSet => _hashSet;
        }
        [Test]
        public void HashSetField_SetsField()
        {
            var fieldName = "_hashSet";

            var fixture = Fixture.New<HashSetClass>().BypassConstructor().WithBackingField(fieldName, c => c.HashSet, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.HashSet.Single(), Is.EqualTo(_text));
        }

        class SortedSetClass
        {
            private readonly SortedSet<string> _sortedSet = null!;
            public IEnumerable<string> SortedSet => _sortedSet;
        }
        [Test]
        public void SortedSetField_SetsField()
        {
            var fieldName = "_sortedSet";

            var fixture = Fixture.New<SortedSetClass>().BypassConstructor().WithBackingField(fieldName, c => c.SortedSet, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.SortedSet.Single(), Is.EqualTo(_text));
        }

        class ImmutableSortedSetClass
        {
            private readonly ImmutableSortedSet<string> _immutableSortedSet = null!;
            public IEnumerable<string> ImmutableSortedSet => _immutableSortedSet;
        }
        [Test]
        public void ImmutableSortedSetField_SetsField()
        {
            var fieldName = "_immutableSortedSet";

            var fixture = Fixture.New<ImmutableSortedSetClass>().BypassConstructor().WithBackingField(fieldName, c => c.ImmutableSortedSet, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ImmutableSortedSet.Single(), Is.EqualTo(_text));
        }

        class ISetClass
        {
            private readonly ISet<string> _iSet = null!;
            public IEnumerable<string> ISet => _iSet;
        }
        [Test]
        public void ISetField_SetsField()
        {
            var fieldName = "_iSet";

            var fixture = Fixture.New<ISetClass>().BypassConstructor().WithBackingField(fieldName, c => c.ISet, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ISet.Single(), Is.EqualTo(_text));
        }

        class IReadOnlySetClass
        {
            private readonly IReadOnlySet<string> _iReadOnlySet = null!;
            public IEnumerable<string> IReadOnlySet => _iReadOnlySet;
        }
        [Test]
        public void IReadOnlySetField_SetsField()
        {
            var fieldName = "_iReadOnlySet";

            var fixture = Fixture.New<IReadOnlySetClass>().BypassConstructor().WithBackingField(fieldName, c => c.IReadOnlySet, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IReadOnlySet.Single(), Is.EqualTo(_text));
        }

        class IImmutableSetClass
        {
            private readonly IImmutableSet<string> _iImmutableSet = null!;
            public IEnumerable<string> IImmutableSet => _iImmutableSet;
        }
        [Test]
        public void IImmutableSetField_SetsField()
        {
            var fieldName = "_iImmutableSet";

            var fixture = Fixture.New<IImmutableSetClass>().BypassConstructor().WithBackingField(fieldName, c => c.IImmutableSet, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IImmutableSet.Single(), Is.EqualTo(_text));
        }

        class ImmutableHashSetClass
        {
            private readonly ImmutableHashSet<string> _immutableHashSet = null!;
            public IEnumerable<string> ImmutableHashSet => _immutableHashSet;
        }
        [Test]
        public void ImmutableHashSetField_SetsField()
        {
            var fieldName = "_immutableHashSet";

            var fixture = Fixture.New<ImmutableHashSetClass>().BypassConstructor().WithBackingField(fieldName, c => c.ImmutableHashSet, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ImmutableHashSet.Single(), Is.EqualTo(_text));
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

            var fixture = Fixture.New<FrozenSetClass>().BypassConstructor().WithBackingField(fieldName, c => c.FrozenSet, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.FrozenSet.Single(), Is.EqualTo(_text));
        }
        #endregion

        #region Collections
        class CollectionClass
        {
            private readonly Collection<string> _collection = null!;
            public IEnumerable<string> Collection => _collection;
        }
        [Test]
        public void CollectionField_SetsField()
        {
            var fieldName = "_collection";

            var fixture = Fixture.New<CollectionClass>().BypassConstructor().WithBackingField(fieldName, c => c.Collection, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Collection.Single(), Is.EqualTo(_text));
        }

        class ReadOnlyCollectionClass
        {
            private readonly ReadOnlyCollection<string> _readOnlyCollection = null!;
            public IEnumerable<string> ReadOnlyCollection => _readOnlyCollection;
        }
        [Test]
        public void ReadOnlyCollectionField_SetsField()
        {
            var fieldName = "_readOnlyCollection";

            var fixture = Fixture.New<ReadOnlyCollectionClass>().BypassConstructor().WithBackingField(fieldName, c => c.ReadOnlyCollection, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ReadOnlyCollection.Single(), Is.EqualTo(_text));
        }

        class IReadOnlyCollectionClass
        {
            private readonly IReadOnlyCollection<string> _iReadOnlyCollection = null!;
            public ImmutableQueue<string> IReadOnlyCollection => [.. _iReadOnlyCollection];
        }
        [Test]
        public void IReadOnlyCollectionField_SetsField()
        {
            var fieldName = "_iReadOnlyCollection";

            var fixture = Fixture.New<IReadOnlyCollectionClass>().BypassConstructor().WithBackingField(fieldName, c => c.IReadOnlyCollection, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IReadOnlyCollection.Single(), Is.EqualTo(_text));
        }

        class ICollectionClass
        {
            private readonly ICollection<string> _iCollection = null!;
            public ImmutableQueue<string> ICollection => [.. _iCollection];
        }
        [Test]
        public void ICollectionField_SetsField()
        {
            var fieldName = "_iCollection";

            var fixture = Fixture.New<ICollectionClass>().BypassConstructor().WithBackingField(fieldName, c => c.ICollection, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ICollection.Single(), Is.EqualTo(_text));
        }

        class ICollectionNonGenericClass
        {
            private readonly ICollection _iCollection = null!;
            public IEnumerable<object> ICollection => _iCollection.Cast<object>();
        }
        [Test]
        public void ICollectionNonGenericField_SetsField()
        {
            var fieldName = "_iCollection";

            var fixture = Fixture.New<ICollectionNonGenericClass>().BypassConstructor().WithBackingField(fieldName, c => c.ICollection, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ICollection.Single(), Is.EqualTo(_text));
        }

        class ICollectionNonGenericStackClass
        {
            private readonly ICollection _iCollection = null!;
            public ArrayList ICollection => new(_iCollection);
        }
        [Test]
        public void ICollectionNonGenericField_StackProperty_SetsField()
        {
            var fieldName = "_iCollection";

            var fixture = Fixture.New<ICollectionNonGenericStackClass>().BypassConstructor().WithBackingField(fieldName, c => c.ICollection, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ICollection.ToArray().First(), Is.EqualTo(_text));
        }

        class BlockingCollectionClass
        {
            private readonly BlockingCollection<string> _collection = null!;
            public IEnumerable<string> Collection => _collection;
        }
        [Test]
        [Ignore("Not yet supported, will research if it should be.")]
        public void BlockingCollectionField_SetsField()
        {
            var fieldName = "_collection";

            var fixture = Fixture.New<BlockingCollectionClass>().BypassConstructor().WithBackingField(fieldName, c => c.Collection, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Collection.Single(), Is.EqualTo(_text));
        }
        #endregion

        #region Stacks
        class StackClass
        {
            private readonly Stack<string> _stack = null!;
            public IEnumerable<string> Stack => _stack;
        }
        [Test]
        public void StackField_SetsField()
        {
            var fieldName = "_stack";

            var fixture = Fixture.New<StackClass>().BypassConstructor().WithBackingField(fieldName, c => c.Stack, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Stack.Single(), Is.EqualTo(_text));
        }

        class ImmutableStackClass
        {
            private readonly ImmutableStack<string> _immutableStack = null!;
            public IEnumerable<string> ImmutableStack => _immutableStack;
        }
        [Test]
        public void ImmutableStackField_SetsField()
        {
            var fieldName = "_immutableStack";

            var fixture = Fixture.New<ImmutableStackClass>().BypassConstructor().WithBackingField(fieldName, c => c.ImmutableStack, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ImmutableStack.Single(), Is.EqualTo(_text));
        }

        class IImmutableStackClass
        {
            private readonly IImmutableStack<string> _iImmutableStack = null!;
            public IEnumerable<string> IImmutableStack => _iImmutableStack;
        }
        [Test]
        public void IImmutableStackField_SetsField()
        {
            var fieldName = "_iImmutableStack";

            var fixture = Fixture.New<IImmutableStackClass>().BypassConstructor().WithBackingField(fieldName, c => c.IImmutableStack, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IImmutableStack.Single(), Is.EqualTo(_text));
        }

        class StackNonGenericClass
        {
            private readonly Stack _stack = null!;
            public IEnumerable<object> Stack => _stack.Cast<object>();
        }
        [Test]
        public void StackNonGenericField_SetsField()
        {
            var fieldName = "_stack";

            var fixture = Fixture.New<StackNonGenericClass>().BypassConstructor().WithBackingField(fieldName, c => c.Stack, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Stack.Single(), Is.EqualTo(_text));
        }

        class StackNonGenericArrayListClass
        {
            private readonly Stack _stack = null!;
            public ArrayList Stack => new(_stack);
        }
        [Test]
        public void StackNonGenericField_ArrayListProperty_SetsField()
        {
            var fieldName = "_stack";

            var fixture = Fixture.New<StackNonGenericArrayListClass>().BypassConstructor().WithBackingField(fieldName, c => c.Stack, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Stack.ToArray().First(), Is.EqualTo(_text));
        }

        class ConcurrentStackClass
        {
            private readonly ConcurrentStack<string> _concurrentStack = null!;
            public IEnumerable<string> ConcurrentStack => _concurrentStack;
        }
        [Test]
        public void ConcurrentStackField_SetsField()
        {
            var fieldName = "_concurrentStack";

            var fixture = Fixture.New<ConcurrentStackClass>().BypassConstructor().WithBackingField(fieldName, c => c.ConcurrentStack, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ConcurrentStack.Single(), Is.EqualTo(_text));
        }
        #endregion

        #region Queues
        class QueueClass
        {
            private readonly Queue<string> _queue = null!;
            public IEnumerable<string> Queue => _queue;
        }
        [Test]
        public void QueueField_SetsField()
        {
            var fieldName = "_queue";

            var fixture = Fixture.New<QueueClass>().BypassConstructor().WithBackingField(fieldName, c => c.Queue, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Queue.Single(), Is.EqualTo(_text));
        }

        class ImmutableQueueClass
        {
            private readonly ImmutableQueue<string> _immutableQueue = null!;
            public IEnumerable<string> ImmutableQueue => _immutableQueue;
        }
        [Test]
        public void ImmutableQueueField_SetsField()
        {
            var fieldName = "_immutableQueue";

            var fixture = Fixture.New<ImmutableQueueClass>().BypassConstructor().WithBackingField(fieldName, c => c.ImmutableQueue, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ImmutableQueue.Single(), Is.EqualTo(_text));
        }

        class IImmutableQueueClass
        {
            private readonly IImmutableQueue<string> _iImmutableQueue = null!;
            public IEnumerable<string> IImmutableQueue => _iImmutableQueue;
        }
        [Test]
        public void IImmutableQueueField_SetsField()
        {
            var fieldName = "_iImmutableQueue";

            var fixture = Fixture.New<IImmutableQueueClass>().BypassConstructor().WithBackingField(fieldName, c => c.IImmutableQueue, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IImmutableQueue.Single(), Is.EqualTo(_text));
        }

        class QueueNonGenericClass
        {
            private readonly Queue _queue = null!;
            public IEnumerable<object> Queue => _queue.Cast<object>();
        }
        [Test]
        public void QueueNonGenericField_SetsField()
        {
            var fieldName = "_queue";

            var fixture = Fixture.New<QueueNonGenericClass>().BypassConstructor().WithBackingField(fieldName, c => c.Queue, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Queue.Single(), Is.EqualTo(_text));
        }

        class QueueNonGenericArrayListClass
        {
            private readonly Queue _queue = null!;
            public ArrayList Queue => new(_queue);
        }
        [Test]
        public void QueueNonGenericField_ArrayListProperty_SetsField()
        {
            var fieldName = "_queue";

            var fixture = Fixture.New<QueueNonGenericArrayListClass>().BypassConstructor().WithBackingField(fieldName, c => c.Queue, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Queue.ToArray().Single(), Is.EqualTo(_text));
        }

        class ConcurrentQueueClass
        {
            private readonly ConcurrentQueue<string> _concurrentQueue = null!;
            public IEnumerable<string> ConcurrentQueue => _concurrentQueue;
        }
        [Test]
        public void ConcurrentQueueField_SetsField()
        {
            var fieldName = "_concurrentQueue";

            var fixture = Fixture.New<ConcurrentQueueClass>().BypassConstructor().WithBackingField(fieldName, c => c.ConcurrentQueue, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ConcurrentQueue.Single(), Is.EqualTo(_text));
        }
        #endregion

        #region IEnumerable
        class IEnumerableClass
        {
            private readonly IEnumerable<string> _iEnumerable = null!;
            public ArrayList IEnumerable => new((ICollection)_iEnumerable);
        }
        [Test]
        public void IEnumerableField_SetsField()
        {
            var fieldName = "_iEnumerable";

            var fixture = Fixture.New<IEnumerableClass>().BypassConstructor().WithBackingField(fieldName, c => c.IEnumerable, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IEnumerable.Cast<string>().Single(), Is.EqualTo(_text));
        }

        class IEnumerableNonGenericClass
        {
            private readonly IEnumerable _iEnumerable = null!;
            public IEnumerable<string> IEnumerable => _iEnumerable.Cast<string>();
        }
        [Test]
        public void IEnumerableNonGenericField_SetsField()
        {
            var fieldName = "_iEnumerable";

            var fixture = Fixture.New<IEnumerableNonGenericClass>().BypassConstructor().WithBackingField(fieldName, c => c.IEnumerable, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IEnumerable.Cast<string>().Single(), Is.EqualTo(_text));
        }

        class IEnumerableNonGenericArrayListClass
        {
            private readonly IEnumerable _iEnumerable = null!;
            public ArrayList IEnumerable => new(Enumerable.Cast<object>(_iEnumerable).ToList());
        }
        [Test]
        public void IEnumerableNonGenericField_ArrayListProperty_SetsField()
        {
            var fieldName = "_iEnumerable";

            var fixture = Fixture.New<IEnumerableNonGenericArrayListClass>().BypassConstructor().WithBackingField(fieldName, c => c.IEnumerable, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IEnumerable.ToArray().Single(), Is.EqualTo(_text));
        }
        #endregion

        #region ArrayList
        class ArrayListClass
        {
            private readonly ArrayList _arrayList = null!;
            public IEnumerable<object> ArrayList => _arrayList.Cast<object>();
        }
        [Test]
        public void ArrayListField_SetsField()
        {
            var fieldName = "_arrayList";

            var fixture = Fixture.New<ArrayListClass>().BypassConstructor().WithBackingField(fieldName, c => c.ArrayList, [_number]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ArrayList.Single(), Is.EqualTo(_number));
        }
        #endregion
    }
}
