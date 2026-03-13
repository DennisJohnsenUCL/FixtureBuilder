using FixtureBuilder.FixtureContexts;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.UninitializedProviders.Decorators;
using Moq;

namespace FixtureBuilder.Tests.UninitializedProviders.Decorators
{
    internal sealed class TypeLinkingUninitializedProviderTests
    {
        private Mock<IFixtureUninitializedProvider> _innerMock;
        private Mock<IFixtureContext> _contextMock;
        private TypeLinkingUninitializedProvider _sut;

        [SetUp]
        public void SetUp()
        {
            _innerMock = new Mock<IFixtureUninitializedProvider>();
            _contextMock = new Mock<IFixtureContext>();
            _sut = new TypeLinkingUninitializedProvider(_innerMock.Object);
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
        public void ResolveUninitialized_WhenTypeIsNullable_UnwrapsAndDelegatesToInner()
        {
            var request = new FixtureRequest(typeof(int?));
            var initializeMembers = InitializeMembers.All;
            var expected = new object();

            _contextMock.Setup(c => c.Link(typeof(int))).Returns((Type?)null);
            _innerMock.Setup(i => i.ResolveUninitialized(request, initializeMembers, _contextMock.Object))
                .Returns(expected);

            var result = _sut.ResolveUninitialized(request, initializeMembers, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(request.Type, Is.EqualTo(typeof(int)));
                Assert.That(result, Is.SameAs(expected));
            }
            _innerMock.Verify(i => i.ResolveUninitialized(request, initializeMembers, _contextMock.Object), Times.Once);
        }

        [Test]
        public void ResolveUninitialized_WhenContextReturnsLink_ReplacesTypeWithLink()
        {
            var request = new FixtureRequest(typeof(IEnumerable<int>));
            var initializeMembers = InitializeMembers.All;
            var expected = new List<int>();

            _contextMock.Setup(c => c.Link(typeof(IEnumerable<int>))).Returns(typeof(List<int>));
            _innerMock.Setup(i => i.ResolveUninitialized(request, initializeMembers, _contextMock.Object))
                .Returns(expected);

            var result = _sut.ResolveUninitialized(request, initializeMembers, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(request.Type, Is.EqualTo(typeof(List<int>)));
                Assert.That(result, Is.SameAs(expected));
            }
        }

        [Test]
        public void ResolveUninitialized_WhenNullableAndLinkExists_UnwrapsThenAppliesLink()
        {
            var request = new FixtureRequest(typeof(int?));
            var initializeMembers = InitializeMembers.All;
            var expected = 42L;

            _contextMock.Setup(c => c.Link(typeof(int))).Returns(typeof(long));
            _innerMock.Setup(i => i.ResolveUninitialized(request, initializeMembers, _contextMock.Object))
                .Returns(expected);

            var result = _sut.ResolveUninitialized(request, initializeMembers, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(request.Type, Is.EqualTo(typeof(long)));
                Assert.That(result, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ResolveUninitialized_WhenNonNullableAndNoLink_DelegatesToInnerUnchanged()
        {
            var request = new FixtureRequest(typeof(string));
            var initializeMembers = InitializeMembers.All;
            var expected = "hello";

            _contextMock.Setup(c => c.Link(typeof(string))).Returns((Type?)null);
            _innerMock.Setup(i => i.ResolveUninitialized(request, initializeMembers, _contextMock.Object))
                .Returns(expected);

            var result = _sut.ResolveUninitialized(request, initializeMembers, _contextMock.Object);

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

            var result = _sut.ResolveUninitialized(request, initializeMembers, _contextMock.Object);

            Assert.That(result, Is.Null);
        }
    }
}
