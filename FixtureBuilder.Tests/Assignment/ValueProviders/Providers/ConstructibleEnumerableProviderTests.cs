using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
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

        [TestCase(typeof(List<int>), TestName = "Resolve_List_ReturnsEmptyInstance")]
        [TestCase(typeof(Stack<string>), TestName = "Resolve_GenericStack_ReturnsEmptyInstance")]
        [TestCase(typeof(Queue<double>), TestName = "Resolve_GenericQueue_ReturnsEmptyInstance")]
        [TestCase(typeof(SortedSet<int>), TestName = "Resolve_SortedSet_ReturnsEmptyInstance")]
        [TestCase(typeof(HashSet<int>), TestName = "Resolve_HashSet_ReturnsEmptyInstance")]
        [TestCase(typeof(LinkedList<int>), TestName = "Resolve_LinkedList_ReturnsEmptyInstance")]
        [TestCase(typeof(Collection<int>), TestName = "Resolve_Collection_ReturnsEmptyInstance")]
        [TestCase(typeof(ConcurrentBag<int>), TestName = "Resolve_ConcurrentBag_ReturnsEmptyInstance")]
        [TestCase(typeof(ConcurrentQueue<int>), TestName = "Resolve_ConcurrentQueue_ReturnsEmptyInstance")]
        [TestCase(typeof(ConcurrentStack<int>), TestName = "Resolve_ConcurrentStack_ReturnsEmptyInstance")]
        [TestCase(typeof(Dictionary<string, int>), TestName = "Resolve_Dictionary_ReturnsEmptyInstance")]
        [TestCase(typeof(ConcurrentDictionary<string, int>), TestName = "Resolve_ConcurrentDictionary_ReturnsEmptyInstance")]
        [TestCase(typeof(OrderedDictionary<string, int>), TestName = "Resolve_OrderedDictionary_ReturnsEmptyInstance")]
        [TestCase(typeof(SortedDictionary<string, int>), TestName = "Resolve_SortedDictionary_ReturnsEmptyInstance")]
        [TestCase(typeof(SortedList<string, int>), TestName = "Resolve_GenericSortedList_ReturnsEmptyInstance")]
        [TestCase(typeof(ArrayList), TestName = "Resolve_ArrayList_ReturnsEmptyInstance")]
        [TestCase(typeof(Stack), TestName = "Resolve_NonGenericStack_ReturnsEmptyInstance")]
        [TestCase(typeof(Queue), TestName = "Resolve_NonGenericQueue_ReturnsEmptyInstance")]
        [TestCase(typeof(Hashtable), TestName = "Resolve_Hashtable_ReturnsEmptyInstance")]
        [TestCase(typeof(SortedList), TestName = "Resolve_NonGenericSortedList_ReturnsEmptyInstance")]
        public void Resolve_SupportedCollectionType_ReturnsEmptyInstance(Type collectionType)
        {
            var request = new FixtureRequest(collectionType);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf(collectionType));
                Assert.That((IEnumerable)result!, Is.Empty);
            }
        }

        [TestCase(typeof(ImmutableList<int>), TestName = "Resolve_ImmutableList_ReturnsNoResult")]
        [TestCase(typeof(string), TestName = "Resolve_PlainClass_ReturnsNoResult")]
        [TestCase(typeof(IList<int>), TestName = "Resolve_Interface_ReturnsNoResult")]
        public void Resolve_UnsupportedType_ReturnsNoResult(Type type)
        {
            var request = new FixtureRequest(type);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
