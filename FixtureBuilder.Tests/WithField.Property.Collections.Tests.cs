using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace FixtureBuilder.Tests
{
    internal sealed partial class WithFieldTests
    {
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

            var fixture = Fixture.New<ListClass>().BypassConstructor().WithField(fieldName, c => c.List, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.List.Single(), Is.EqualTo(_text));
        }

        class IReadOnlyListClass
        {
            private readonly IReadOnlyList<string> _iReadOnlyList = null!;
            public IEnumerable<string> IReadOnlyList => _iReadOnlyList;
        }
        [Test]
        public void IReadOnlyListField_SetsField()
        {
            var fieldName = "_iReadOnlyList";

            var fixture = Fixture.New<IReadOnlyListClass>().BypassConstructor().WithField(fieldName, c => c.IReadOnlyList, [_text]);
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

            var fixture = Fixture.New<ImmutableListClass>().BypassConstructor().WithField(fieldName, c => c.ImmutableList, [_text]);
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

            var fixture = Fixture.New<IImmutableListClass>().BypassConstructor().WithField(fieldName, c => c.IImmutableList, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IImmutableList.Single(), Is.EqualTo(_text));
        }

        class IListClass
        {
            private readonly IList<string> _iList = null!;
            public IEnumerable<string> IList => _iList;
        }
        [Test]
        public void IListField_SetsField()
        {
            var fieldName = "_iList";

            var fixture = Fixture.New<IListClass>().BypassConstructor().WithField(fieldName, c => c.IList, [_text]);
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

            var fixture = Fixture.New<IListNonGenericClass>().BypassConstructor().WithField(fieldName, c => c.IList, [_text]);
            var field = Helpers.GetFixture(fixture);
            Assert.That(field.IList.Single(), Is.EqualTo(_text));
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

            var fixture = Fixture.New<ArrayClass>().BypassConstructor().WithField(fieldName, c => c.StringArray, [_text]);
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

            var fixture = Fixture.New<ImmutableArrayClass>().BypassConstructor().WithField(fieldName, c => c.ImmutableArray, [_text]);
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

            var fixture = Fixture.New<HashSetClass>().BypassConstructor().WithField(fieldName, c => c.HashSet, [_text]);
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

            var fixture = Fixture.New<SortedSetClass>().BypassConstructor().WithField(fieldName, c => c.SortedSet, [_text]);
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

            var fixture = Fixture.New<ImmutableSortedSetClass>().BypassConstructor().WithField(fieldName, c => c.ImmutableSortedSet, [_text]);
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

            var fixture = Fixture.New<ISetClass>().BypassConstructor().WithField(fieldName, c => c.ISet, [_text]);
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

            var fixture = Fixture.New<IReadOnlySetClass>().BypassConstructor().WithField(fieldName, c => c.IReadOnlySet, [_text]);
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

            var fixture = Fixture.New<IImmutableSetClass>().BypassConstructor().WithField(fieldName, c => c.IImmutableSet, [_text]);
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

            var fixture = Fixture.New<ImmutableHashSetClass>().BypassConstructor().WithField(fieldName, c => c.ImmutableHashSet, [_text]);
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

            var fixture = Fixture.New<FrozenSetClass>().BypassConstructor().WithField(fieldName, c => c.FrozenSet, [_text]);
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

            var fixture = Fixture.New<CollectionClass>().BypassConstructor().WithField(fieldName, c => c.Collection, [_text]);
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

            var fixture = Fixture.New<ReadOnlyCollectionClass>().BypassConstructor().WithField(fieldName, c => c.ReadOnlyCollection, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ReadOnlyCollection.Single(), Is.EqualTo(_text));
        }

        class IReadOnlyCollectionClass
        {
            private readonly IReadOnlyCollection<string> _iReadOnlyCollection = null!;
            public IEnumerable<string> IReadOnlyCollection => _iReadOnlyCollection;
        }
        [Test]
        public void IReadOnlyCollectionField_SetsField()
        {
            var fieldName = "_iReadOnlyCollection";

            var fixture = Fixture.New<IReadOnlyCollectionClass>().BypassConstructor().WithField(fieldName, c => c.IReadOnlyCollection, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IReadOnlyCollection.Single(), Is.EqualTo(_text));
        }

        class ICollectionClass
        {
            private readonly ICollection<string> _iCollection = null!;
            public IEnumerable<string> ICollection => _iCollection;
        }
        [Test]
        public void ICollectionField_SetsField()
        {
            var fieldName = "_iCollection";

            var fixture = Fixture.New<ICollectionClass>().BypassConstructor().WithField(fieldName, c => c.ICollection, [_text]);
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

            var fixture = Fixture.New<ICollectionNonGenericClass>().BypassConstructor().WithField(fieldName, c => c.ICollection, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ICollection.Single(), Is.EqualTo(_text));
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

            var fixture = Fixture.New<StackClass>().BypassConstructor().WithField(fieldName, c => c.Stack, [_text]);
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

            var fixture = Fixture.New<ImmutableStackClass>().BypassConstructor().WithField(fieldName, c => c.ImmutableStack, [_text]);
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

            var fixture = Fixture.New<IImmutableStackClass>().BypassConstructor().WithField(fieldName, c => c.IImmutableStack, [_text]);
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

            var fixture = Fixture.New<StackNonGenericClass>().BypassConstructor().WithField(fieldName, c => c.Stack, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Stack.Single(), Is.EqualTo(_text));
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

            var fixture = Fixture.New<QueueClass>().BypassConstructor().WithField(fieldName, c => c.Queue, [_text]);
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

            var fixture = Fixture.New<ImmutableQueueClass>().BypassConstructor().WithField(fieldName, c => c.ImmutableQueue, [_text]);
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

            var fixture = Fixture.New<IImmutableQueueClass>().BypassConstructor().WithField(fieldName, c => c.IImmutableQueue, [_text]);
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

            var fixture = Fixture.New<QueueNonGenericClass>().BypassConstructor().WithField(fieldName, c => c.Queue, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Queue.Single(), Is.EqualTo(_text));
        }
        #endregion

        #region Dictionaries
        //class DictionaryClass
        //{
        //    private readonly Dictionary<int, string> _dictionary = null!;
        //    public List<KeyValuePair<int, string>> Dictionary => [.. _dictionary];
        //}
        //[Test]
        //public void DictionaryField_SetsField()
        //{
        //    var fieldName = "_dictionary";
        //    var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

        //    var fixture = Fixture.New<DictionaryClass>().BypassConstructor().WithField(fieldName, c => c.Dictionary, dictionary);
        //    var field = Helpers.GetFixture(fixture);

        //    Assert.That(field.Dictionary.Single().Value, Is.EqualTo("test"));
        //}

        //class IDictionaryNonGenericClass
        //{
        //    private readonly IDictionary _iDictionary = null!;
        //    public List<KeyValuePair<int, string>> IDictionary => [.. _iDictionary.Cast<KeyValuePair<int, string>>()];
        //}
        //[Test]
        //public void IDictionaryNonGenericField_SetsField()
        //{
        //    var fieldName = "_iDictionary";
        //    var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

        //    var fixture = Fixture.New<IDictionaryNonGenericClass>().BypassConstructor().WithField(fieldName, c => c.IDictionary, dictionary);
        //    var field = Helpers.GetFixture(fixture);

        //    Assert.That(field.IDictionary.Cast<KeyValuePair<int, string>>().Single().Value, Is.EqualTo(_text));
        //}

        //class IDictionaryClass
        //{
        //    private readonly IDictionary<int, string> _iDictionary = null!;
        //    public List<KeyValuePair<int, string>> IDictionary => [.. _iDictionary];
        //}
        //[Test]
        //public void IDictionaryField_SetsField()
        //{
        //    var fieldName = "_iDictionary";
        //    var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

        //    var fixture = Fixture.New<IDictionaryClass>().BypassConstructor().WithField(fieldName, c => c.IDictionary, dictionary);
        //    var field = Helpers.GetFixture(fixture);

        //    Assert.That(field.IDictionary.Single().Value, Is.EqualTo(_text));
        //}

        //class ImmutableDictionaryClass
        //{
        //    private readonly ImmutableDictionary<int, string> _immutableDictionary = null!;
        //    public List<KeyValuePair<int, string>> ImmutableDictionary => [.. _immutableDictionary];
        //}
        //[Test]
        //public void ImmutableDictionaryField_SetsField()
        //{
        //    var fieldName = "_immutableDictionary";
        //    var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

        //    var fixture = Fixture.New<ImmutableDictionaryClass>().BypassConstructor().WithField(fieldName, c => c.ImmutableDictionary, dictionary);
        //    var field = Helpers.GetFixture(fixture);

        //    Assert.That(field.ImmutableDictionary.Single().Value, Is.EqualTo(_text));
        //}

        //class IImmutableDictionaryClass
        //{
        //    private readonly IImmutableDictionary<int, string> _iImmutableDictionary = null!;
        //    public List<KeyValuePair<int, string>> IImmutableDictionary => [.. _iImmutableDictionary];
        //}
        //[Test]
        //public void IImmutableDictionaryField_SetsField()
        //{
        //    var fieldName = "_iImmutableDictionary";
        //    var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

        //    var fixture = Fixture.New<IImmutableDictionaryClass>().BypassConstructor().WithField(fieldName, c => c.IImmutableDictionary, dictionary);
        //    var field = Helpers.GetFixture(fixture);

        //    Assert.That(field.IImmutableDictionary.Single().Value, Is.EqualTo(_text));
        //}

        //class SortedDictionaryClass
        //{
        //    private readonly SortedDictionary<int, string> _sortedDictionary = null!;
        //    public List<KeyValuePair<int, string>> SortedDictionary => [.. _sortedDictionary];
        //}
        //[Test]
        //public void SortedDictionaryField_SetsField()
        //{
        //    var fieldName = "_sortedDictionary";
        //    var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

        //    var fixture = Fixture.New<SortedDictionaryClass>().BypassConstructor().WithField(fieldName, c => c.SortedDictionary, dictionary);
        //    var field = Helpers.GetFixture(fixture);

        //    Assert.That(field.SortedDictionary.Single().Value, Is.EqualTo(_text));
        //}

        //class OrderedDictionaryClass
        //{
        //    private readonly OrderedDictionary<int, string> _orderedDictionary = null!;
        //    public List<KeyValuePair<int, string>> OrderedDictionary => [.. _orderedDictionary];
        //}
        //[Test]
        //public void OrderedDictionaryField_SetsField()
        //{
        //    var fieldName = "_orderedDictionary";
        //    var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

        //    var fixture = Fixture.New<OrderedDictionaryClass>().BypassConstructor().WithField(fieldName, c => c.OrderedDictionary, dictionary);
        //    var field = Helpers.GetFixture(fixture);

        //    Assert.That(field.OrderedDictionary.Single().Value, Is.EqualTo(_text));
        //}

        //class ImmutableSortedDictionaryClass
        //{
        //    private readonly ImmutableSortedDictionary<int, string> _immutableSortedDictionary = null!;
        //    public List<KeyValuePair<int, string>> ImmutableSortedDictionary => [.. _immutableSortedDictionary];
        //}
        //[Test]
        //public void ImmutableSortedDictionaryField_SetsField()
        //{
        //    var fieldName = "_immutableSortedDictionary";
        //    var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

        //    var fixture = Fixture.New<ImmutableSortedDictionaryClass>().BypassConstructor().WithField(fieldName, c => c.ImmutableSortedDictionary, dictionary);
        //    var field = Helpers.GetFixture(fixture);

        //    Assert.That(field.ImmutableSortedDictionary.Single().Value, Is.EqualTo(_text));
        //}

        //class FrozenDictionaryClass
        //{
        //    private readonly FrozenDictionary<int, string> _frozenDictionary = null!;
        //    public List<KeyValuePair<int, string>> FrozenDictionary => [.. _frozenDictionary];
        //}
        //[Test]
        //public void FrozenDictionaryField_SetsField()
        //{
        //    var fieldName = "_frozenDictionary";
        //    var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

        //    var fixture = Fixture.New<FrozenDictionaryClass>().BypassConstructor().WithField(fieldName, c => c.FrozenDictionary, dictionary);
        //    var field = Helpers.GetFixture(fixture);

        //    Assert.That(field.FrozenDictionary.Single().Value, Is.EqualTo(_text));
        //}

        //class ReadOnlyDictionaryClass
        //{
        //    private readonly ReadOnlyDictionary<int, string> _readOnlyDictionary = null!;
        //    public List<KeyValuePair<int, string>> ReadOnlyDictionary => [.. _readOnlyDictionary];
        //}
        //[Test]
        //public void ReadOnlyDictionaryField_SetsField()
        //{
        //    var fieldName = "_readOnlyDictionary";
        //    var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

        //    var fixture = Fixture.New<ReadOnlyDictionaryClass>().BypassConstructor().WithField(fieldName, c => c.ReadOnlyDictionary, dictionary);
        //    var field = Helpers.GetFixture(fixture);

        //    Assert.That(field.ReadOnlyDictionary.Single().Value, Is.EqualTo(_text));
        //}

        //class IReadOnlyDictionaryClass
        //{
        //    private readonly IReadOnlyDictionary<int, string> _iReadOnlyDictionary = null!;
        //    public List<KeyValuePair<int, string>> IReadOnlyDictionary => [.. _iReadOnlyDictionary];
        //}
        //[Test]
        //public void IReadOnlyDictionaryField_SetsField()
        //{
        //    var fieldName = "_iReadOnlyDictionary";
        //    var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

        //    var fixture = Fixture.New<IReadOnlyDictionaryClass>().BypassConstructor().WithField(fieldName, c => c.IReadOnlyDictionary, dictionary);
        //    var field = Helpers.GetFixture(fixture);

        //    Assert.That(field.IReadOnlyDictionary.Single().Value, Is.EqualTo(_text));
        //}

        //class SortedListClass
        //{
        //    private readonly SortedList<int, string> _sortedList = null!;
        //    public List<KeyValuePair<int, string>> SortedList => [.. _sortedList];
        //}
        //[Test]
        //public void SortedListField_SetsField()
        //{
        //    var fieldName = "_sortedList";
        //    var sortedList = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

        //    var fixture = Fixture.New<SortedListClass>().BypassConstructor().WithField(fieldName, c => c.SortedList, sortedList);
        //    var field = Helpers.GetFixture(fixture);

        //    Assert.That(field.SortedList.Single().Value, Is.EqualTo(_text));
        //}

        //class SortedListNonGenericClass
        //{
        //    private readonly SortedList _sortedList = null!;
        //    public List<KeyValuePair<int, string>> SortedList => [.. _sortedList.Cast<KeyValuePair<int, string>>()];
        //}
        //[Test]
        //public void SortedListNonGenericField_SetsField()
        //{
        //    var fieldName = "_sortedList";
        //    var sortedList = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

        //    var fixture = Fixture.New<SortedListNonGenericClass>().BypassConstructor().WithField(fieldName, c => c.SortedList, sortedList);
        //    var field = Helpers.GetFixture(fixture);

        //    Assert.That(field.SortedList.Cast<KeyValuePair<object, object>>().Single().Value, Is.EqualTo(_text));
        //}

        //class HashtableClass
        //{
        //    private readonly Hashtable _hashtable = null!;
        //    public List<KeyValuePair<int, string>> Hashtable => [.. _hashtable.Cast<KeyValuePair<int, string>>()];
        //}
        //[Test]
        //public void HashtableField_SetsField()
        //{
        //    var fieldName = "_hashtable";
        //    var dictionary = new List<KeyValuePair<int, string>>([new KeyValuePair<int, string>(1, _text)]);

        //    var fixture = Fixture.New<HashtableClass>().BypassConstructor().WithField(fieldName, c => c.Hashtable, dictionary);
        //    var field = Helpers.GetFixture(fixture);

        //    Assert.That(field.Hashtable.Single().Value, Is.EqualTo(_text));
        //}
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

            var fixture = Fixture.New<IEnumerableClass>().BypassConstructor().WithField(fieldName, c => c.IEnumerable, [_text]);
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

            var fixture = Fixture.New<IEnumerableNonGenericClass>().BypassConstructor().WithField(fieldName, c => c.IEnumerable, [_text]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.IEnumerable.Cast<string>().Single(), Is.EqualTo(_text));
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

            var fixture = Fixture.New<ArrayListClass>().BypassConstructor().WithField(fieldName, c => c.ArrayList, [_number]);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.ArrayList.Single(), Is.EqualTo(_number));
        }
        #endregion
    }
}
