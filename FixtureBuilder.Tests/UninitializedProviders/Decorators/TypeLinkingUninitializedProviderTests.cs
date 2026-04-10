using FixtureBuilder.FixtureContexts;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.UninitializedProviders.Decorators;
using Moq;

namespace FixtureBuilder.Tests.UninitializedProviders.Decorators
{
    internal sealed class TypeLinkingUninitializedProviderTests
    {
        private Mock<IUninitializedProvider> _innerMock;
        private Mock<IFixtureContext> _contextMock;
        private TypeLinkingUninitializedProvider _sut;
        private RecursiveResolveContext _recursiveResolveContext;

        [SetUp]
        public void SetUp()
        {
            _innerMock = new Mock<IUninitializedProvider>();
            _contextMock = new Mock<IFixtureContext>();
            _sut = new TypeLinkingUninitializedProvider(_innerMock.Object);
            _recursiveResolveContext = new();
        }

        [Test]
        public void Constructor_WhenInnerIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TypeLinkingUninitializedProvider(null!));
        }

        [Test]
        public void Constructor_WhenInnerIsValid_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new TypeLinkingUninitializedProvider(_innerMock.Object));
        }

        [Test]
        public void ResolveUninitialized_WhenContextReturnsLink_ReplacesTypeWithLink()
        {
            var request = new FixtureRequest(typeof(IEnumerable<int>));
            var initializeMembers = InitializeMembers.All;
            var expected = new List<int>();

            _contextMock.Setup(c => c.Link(typeof(IEnumerable<int>))).Returns(typeof(List<int>));
            _innerMock.Setup(i => i.ResolveUninitialized(request, initializeMembers, _contextMock.Object, _recursiveResolveContext))
                .Returns(expected);

            var result = _sut.ResolveUninitialized(request, initializeMembers, _contextMock.Object, _recursiveResolveContext);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(request.Type, Is.EqualTo(typeof(List<int>)));
                Assert.That(result, Is.SameAs(expected));
            }
        }

        [Test]
        public void ResolveUninitialized_WhenNonNullableAndNoLink_DelegatesToInnerUnchanged()
        {
            var request = new FixtureRequest(typeof(string));
            var initializeMembers = InitializeMembers.All;
            var expected = "hello";

            _contextMock.Setup(c => c.Link(typeof(string))).Returns((Type?)null);
            _innerMock.Setup(i => i.ResolveUninitialized(request, initializeMembers, _contextMock.Object, _recursiveResolveContext))
                .Returns(expected);

            var result = _sut.ResolveUninitialized(request, initializeMembers, _contextMock.Object, _recursiveResolveContext);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(request.Type, Is.EqualTo(typeof(string)));
                Assert.That(result, Is.SameAs(expected));
            }
        }

        [Test]
        public void ResolveUninitialized_WhenInnerReturnsNull_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(string));
            var initializeMembers = InitializeMembers.All;

            _contextMock.Setup(c => c.Link(typeof(string))).Returns((Type?)null);
            _innerMock.Setup(i => i.ResolveUninitialized(request, initializeMembers, _contextMock.Object))
                .Returns((object?)null);

            var result = _sut.ResolveUninitialized(request, initializeMembers, _contextMock.Object, _recursiveResolveContext);

            Assert.That(result, Is.Null);
        }
    }
}
