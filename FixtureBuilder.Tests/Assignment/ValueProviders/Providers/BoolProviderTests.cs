using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class BoolProviderTests
    {
        private BoolProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _sut = new BoolProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new BoolProvider());
        }

        [Test]
        public void Resolve_Bool_ReturnsTrue()
        {
            var request = new FixtureRequest(typeof(bool));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<bool>());
                Assert.That(result, Is.True);
            }
        }

        [Test]
        public void Resolve_Bool_ReturnsSameValuePerCall()
        {
            var request = new FixtureRequest(typeof(bool));

            var first = _sut.ResolveValue(request, _contextMock.Object);
            var second = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(first, Is.EqualTo(second));
        }

        [Test]
        public void Resolve_String_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(string));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Resolve_NullableBool_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(bool?));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Resolve_Int_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(int));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
