using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class CharProviderTests
    {
        private CharProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _sut = new CharProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new CharProvider());
        }

        [Test]
        public void Resolve_Char_ReturnsAsciiLetter()
        {
            var request = new FixtureRequest(typeof(char));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<char>());
                Assert.That(char.IsAsciiLetter((char)result!), Is.True);
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
        public void Resolve_NullableChar_ReturnsNoResult()
        {
            var request = new FixtureRequest(typeof(char?));

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
