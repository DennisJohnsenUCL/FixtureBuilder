using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.ValueProviders.Providers;
using Moq;

namespace FixtureBuilder.Tests.FixtureProviders.Providers
{
    internal sealed class ConstructibleEnumerableProviderTests
    {
        private ConstructibleEnumerableProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _sut = new ConstructibleEnumerableProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new ConstructibleEnumerableProvider());
        }

        // --- Generic single-element collections ---

        [Test]
        public void Resolve_List_ReturnsEmptyListInstance()
        {
            var request = new FixtureRequest(typeof(List<int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<List<int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_Stack_ReturnsEmptyStackInstance()
        {
            var request = new FixtureRequest(typeof(Stack<string>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<Stack<string>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_Queue_ReturnsEmptyQueueInstance()
        {
            var request = new FixtureRequest(typeof(Queue<double>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<Queue<double>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_SortedSet_ReturnsEmptySortedSetInstance()
        {
            var request = new FixtureRequest(typeof(SortedSet<int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<SortedSet<int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_HashSet_ReturnsEmptyHashSetInstance()
        {
            var request = new FixtureRequest(typeof(HashSet<int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<HashSet<int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_LinkedList_ReturnsEmptyLinkedListInstance()
        {
            var request = new FixtureRequest(typeof(LinkedList<int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<LinkedList<int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_Collection_ReturnsEmptyCollectionInstance()
        {
            var request = new FixtureRequest(typeof(Collection<int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<Collection<int>>());
                Assert.That(result, Is.Empty);
            }
        }

        // --- Concurrent collections ---

        [Test]
        public void Resolve_ConcurrentBag_ReturnsEmptyConcurrentBagInstance()
        {
            var request = new FixtureRequest(typeof(ConcurrentBag<int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<ConcurrentBag<int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_ConcurrentQueue_ReturnsEmptyConcurrentQueueInstance()
        {
            var request = new FixtureRequest(typeof(ConcurrentQueue<int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<ConcurrentQueue<int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_ConcurrentStack_ReturnsEmptyConcurrentStackInstance()
        {
            var request = new FixtureRequest(typeof(ConcurrentStack<int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<ConcurrentStack<int>>());
                Assert.That(result, Is.Empty);
            }
        }

        // --- Generic dictionary types ---

        [Test]
        public void Resolve_Dictionary_ReturnsEmptyDictionaryInstance()
        {
            var request = new FixtureRequest(typeof(Dictionary<string, int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<Dictionary<string, int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_ConcurrentDictionary_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(ConcurrentDictionary<string, int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<ConcurrentDictionary<string, int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_OrderedDictionary_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(OrderedDictionary<string, int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<OrderedDictionary<string, int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_SortedDictionary_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(SortedDictionary<string, int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<SortedDictionary<string, int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_GenericSortedList_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(SortedList<string, int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<SortedList<string, int>>());
                Assert.That(result, Is.Empty);
            }
        }

        // --- Non-generic collections ---

        [Test]
        public void Resolve_ArrayList_ReturnsEmptyArrayListInstance()
        {
            var request = new FixtureRequest(typeof(ArrayList));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<ArrayList>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_NonGenericStack_ReturnsEmptyStackInstance()
        {
            var request = new FixtureRequest(typeof(Stack));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<Stack>());
                Assert.That((Stack)result!, Is.Empty);
            }
        }

        [Test]
        public void Resolve_NonGenericQueue_ReturnsEmptyQueueInstance()
        {
            var request = new FixtureRequest(typeof(Queue));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<Queue>());
                Assert.That((Queue)result!, Is.Empty);
            }
        }

        [Test]
        public void Resolve_Hashtable_ReturnsEmptyHashtableInstance()
        {
            var request = new FixtureRequest(typeof(Hashtable));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<Hashtable>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_NonGenericSortedList_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(SortedList));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<SortedList>());
                Assert.That(result, Is.Empty);
            }
        }

        // --- Unsupported types ---

        [Test]
        public void Resolve_UnsupportedGenericType_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(ImmutableList<int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_PlainClass_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(string));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_Interface_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(IList<int>));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }
    }
}
