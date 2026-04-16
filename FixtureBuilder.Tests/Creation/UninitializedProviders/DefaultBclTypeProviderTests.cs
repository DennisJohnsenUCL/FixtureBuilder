using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Creation.UninitializedProviders
{
    internal sealed class DefaultBclTypeProviderTests
    {
        private DefaultBclTypeProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _sut = new DefaultBclTypeProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new DefaultBclTypeProvider());
        }

        [Test]
        public void Resolve_SystemInt32_ReturnsDefaultInt()
        {
            var request = new FixtureRequest(typeof(int));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<int>());
                Assert.That(result, Is.Zero);
            }
        }

        [Test]
        public void Resolve_SystemGuid_ReturnsEmptyGuid()
        {
            var request = new FixtureRequest(typeof(Guid));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<Guid>());
                Assert.That(result, Is.EqualTo(Guid.Empty));
            }
        }

        [Test]
        public void Resolve_SystemBoolean_ReturnsDefaultBool()
        {
            var request = new FixtureRequest(typeof(bool));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<bool>());
                Assert.That(result, Is.False);
            }
        }

        [Test]
        public void Resolve_SystemCollectionsGenericList_ReturnsEmptyList()
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
        public void Resolve_SystemTimeSpan_ReturnsDefaultTimeSpan()
        {
            var request = new FixtureRequest(typeof(TimeSpan));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<TimeSpan>());
                Assert.That(result, Is.EqualTo(TimeSpan.Zero));
            }
        }

        [Test]
        public void Resolve_NonSystemNamespace_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(DefaultBclTypeProvider));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Resolve_UserDefinedClass_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(FixtureRequest));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
