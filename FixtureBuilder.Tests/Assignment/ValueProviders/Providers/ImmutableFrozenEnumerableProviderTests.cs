using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class ImmutableFrozenEnumerableProviderTests
    {
        private ImmutableFrozenEnumerableProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _sut = new ImmutableFrozenEnumerableProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new ImmutableFrozenEnumerableProvider());
        }

        [TestCase(typeof(ImmutableList<int>), TestName = "Resolve_ImmutableList_ReturnsEmptyInstance")]
        [TestCase(typeof(ReadOnlyCollection<int>), TestName = "Resolve_ReadOnlyCollection_ReturnsEmptyInstance")]
        [TestCase(typeof(ImmutableHashSet<string>), TestName = "Resolve_ImmutableHashSet_ReturnsEmptyInstance")]
        [TestCase(typeof(ImmutableSortedSet<int>), TestName = "Resolve_ImmutableSortedSet_ReturnsEmptyInstance")]
        [TestCase(typeof(ImmutableStack<int>), TestName = "Resolve_ImmutableStack_ReturnsEmptyInstance")]
        [TestCase(typeof(ImmutableQueue<int>), TestName = "Resolve_ImmutableQueue_ReturnsEmptyInstance")]
        [TestCase(typeof(ImmutableArray<int>), TestName = "Resolve_ImmutableArray_ReturnsEmptyInstance")]
        [TestCase(typeof(ImmutableDictionary<string, int>), TestName = "Resolve_ImmutableDictionary_ReturnsEmptyInstance")]
        [TestCase(typeof(ReadOnlyDictionary<string, int>), TestName = "Resolve_ReadOnlyDictionary_ReturnsEmptyInstance")]
        [TestCase(typeof(ImmutableSortedDictionary<string, int>), TestName = "Resolve_ImmutableSortedDictionary_ReturnsEmptyInstance")]
        public void Resolve_ExactTypedCollection_ReturnsEmptyInstance(Type collectionType)
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

        [TestCase(typeof(FrozenSet<int>), TestName = "Resolve_FrozenSet_ReturnsEmptyInstance")]
        [TestCase(typeof(FrozenDictionary<string, int>), TestName = "Resolve_FrozenDictionary_ReturnsEmptyInstance")]
        public void Resolve_AssignableCollection_ReturnsEmptyInstance(Type collectionType)
        {
            var request = new FixtureRequest(collectionType);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.AssignableTo(collectionType));
                Assert.That((IEnumerable)result!, Is.Empty);
            }
        }

        [TestCase(typeof(List<int>), TestName = "Resolve_MutableList_ReturnsNoResult")]
        [TestCase(typeof(string), TestName = "Resolve_PlainClass_ReturnsNoResult")]
        [TestCase(typeof(int), TestName = "Resolve_NonGenericType_ReturnsNoResult")]
        public void Resolve_UnsupportedType_ReturnsNoResult(Type type)
        {
            var request = new FixtureRequest(type);

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
