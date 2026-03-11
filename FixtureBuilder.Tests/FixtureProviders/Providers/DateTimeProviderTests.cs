using FixtureBuilder.FixtureContexts;
using FixtureBuilder.FixtureProviders.Providers;
using Moq;

namespace FixtureBuilder.Tests.FixtureProviders.Providers
{
    internal sealed class DateTimeProviderTests
    {
        private DateTimeProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _sut = new DateTimeProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new DateTimeProvider());
        }

        [Test]
        public void Resolve_DateTime_ReturnsDateTimeInstance()
        {
            var request = new FixtureRequest(typeof(DateTime));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<DateTime>());
        }

        [Test]
        public void Resolve_DateTime_ReturnsUtcNow()
        {
            var request = new FixtureRequest(typeof(DateTime));
            var before = DateTime.UtcNow;

            var result = (DateTime)_sut.Resolve(request, _contextMock.Object)!;

            var after = DateTime.UtcNow;
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(result, Is.InRange(before, after));
            }
        }

        [Test]
        public void Resolve_String_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(string));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_DateTimeOffset_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(DateTimeOffset));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_NullableDateTime_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(DateTime?));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }
    }
}
