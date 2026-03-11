using FixtureBuilder.FixtureContexts;
using FixtureBuilder.FixtureProviders.Providers;
using Moq;

namespace FixtureBuilder.Tests.FixtureProviders.Providers
{
    internal sealed class GuidProviderTests
    {
        private GuidProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _sut = new GuidProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new GuidProvider());
        }

        [Test]
        public void Resolve_Guid_ReturnsGuidInstance()
        {
            var request = new FixtureRequest(typeof(Guid));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<Guid>());
                Assert.That(result, Is.Not.EqualTo(Guid.Empty));
            }
        }

        [Test]
        public void Resolve_Guid_ReturnsUniqueValuesPerCall()
        {
            var request = new FixtureRequest(typeof(Guid));

            var first = _sut.Resolve(request, _contextMock.Object);
            var second = _sut.Resolve(request, _contextMock.Object);

            Assert.That(first, Is.Not.EqualTo(second));
        }

        [Test]
        public void Resolve_String_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(string));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_NullableGuid_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(Guid?));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }
    }
}
