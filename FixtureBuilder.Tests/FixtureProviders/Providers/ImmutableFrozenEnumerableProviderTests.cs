using System.Collections.Frozen;
using System.Collections.Immutable;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.FixtureProviders.Providers;
using Moq;

namespace FixtureBuilder.Tests.FixtureProviders.Providers
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

        // --- Immutable single-element collections ---

        [Test]
        public void Resolve_ImmutableList_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(ImmutableList<int>));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<ImmutableList<int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_ImmutableHashSet_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(ImmutableHashSet<string>));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<ImmutableHashSet<string>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_ImmutableSortedSet_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(ImmutableSortedSet<int>));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<ImmutableSortedSet<int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_ImmutableStack_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(ImmutableStack<int>));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<ImmutableStack<int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_ImmutableQueue_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(ImmutableQueue<int>));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<ImmutableQueue<int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_ImmutableArray_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(ImmutableArray<int>));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<ImmutableArray<int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_FrozenSet_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(FrozenSet<int>));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.AssignableTo<FrozenSet<int>>());
                Assert.That(result, Is.Empty);
            }
        }

        // --- Immutable dictionary types ---

        [Test]
        public void Resolve_ImmutableDictionary_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(ImmutableDictionary<string, int>));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<ImmutableDictionary<string, int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_ImmutableSortedDictionary_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(ImmutableSortedDictionary<string, int>));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<ImmutableSortedDictionary<string, int>>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_FrozenDictionary_ReturnsEmptyInstance()
        {
            var request = new FixtureRequest(typeof(FrozenDictionary<string, int>));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.AssignableTo<FrozenDictionary<string, int>>());
                Assert.That(result, Is.Empty);
            }
        }

        // --- Unsupported types ---

        [Test]
        public void Resolve_MutableList_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(List<int>));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_PlainClass_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(string));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_NonGenericType_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(int));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }
    }
}
