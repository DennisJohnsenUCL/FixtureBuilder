using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class TimeSpanProviderTests
    {
        private TimeSpanProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _sut = new TimeSpanProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new TimeSpanProvider());
        }

        [Test]
        public void Resolve_TimeSpan_ReturnsTimeSpanInstance()
        {
            var request = new FixtureRequest(typeof(TimeSpan));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<TimeSpan>());
                Assert.That(result, Is.EqualTo(TimeSpan.FromMinutes(30)));
            }
        }

        [Test]
        public void Resolve_TimeSpan_ReturnsSameValuePerCall()
        {
            var request = new FixtureRequest(typeof(TimeSpan));

            var first = _sut.ResolveValue(request, _contextMock.Object);
            var second = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(first, Is.EqualTo(second));
        }

        [Test]
        public void Resolve_DateTime_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(DateTime));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Resolve_NullableTimeSpan_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(TimeSpan?));

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
