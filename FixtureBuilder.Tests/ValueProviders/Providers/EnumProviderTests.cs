using FixtureBuilder.FixtureContexts;
using FixtureBuilder.ValueProviders.Providers;
using Moq;

namespace FixtureBuilder.Tests.FixtureProviders.Providers
{
    internal sealed class EnumProviderTests
    {
        private EnumProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        private enum Color { Red, Green, Blue }

        private enum Priority { Low = 10, Medium = 20, High = 30 }

        [Flags]
        private enum Permissions { None = 0, Read = 1, Write = 2, Execute = 4 }

        [SetUp]
        public void SetUp()
        {
            _sut = new EnumProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new EnumProvider());
        }

        [Test]
        public void Resolve_Enum_ReturnsFirstDefinedValue()
        {
            var request = new FixtureRequest(typeof(Color));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<Color>());
                Assert.That(result, Is.EqualTo(Color.Red));
            }
        }

        [Test]
        public void Resolve_EnumWithCustomValues_ReturnsFirstDefinedValue()
        {
            var request = new FixtureRequest(typeof(Priority));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<Priority>());
                Assert.That(result, Is.EqualTo(Priority.Low));
            }
        }

        [Test]
        public void Resolve_FlagsEnum_ReturnsFirstDefinedValue()
        {
            var request = new FixtureRequest(typeof(Permissions));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<Permissions>());
                Assert.That(result, Is.EqualTo(Permissions.None));
            }
        }

        [Test]
        public void Resolve_String_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(string));

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
