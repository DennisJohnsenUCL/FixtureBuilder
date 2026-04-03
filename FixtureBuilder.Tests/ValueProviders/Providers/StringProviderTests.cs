using FixtureBuilder.FixtureContexts;
using FixtureBuilder.ValueProviders.Providers;
using Moq;

namespace FixtureBuilder.Tests.FixtureProviders.Providers
{
    internal sealed class StringProviderTests
    {
        private StringProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _sut = new StringProvider();
            _contextMock = new Mock<IFixtureContext>();
        }

        [Test]
        public void DefaultConstructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new StringProvider());
        }

        [Test]
        public void Resolve_StringWithName_ReturnsName()
        {
            var request = new FixtureRequest(typeof(string), "FirstName", this);

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<string>());
                Assert.That(result, Is.EqualTo("FirstName"));
            }
        }

        [Test]
        public void Resolve_StringWithNullName_ReturnsEmptyString()
        {
            var request = new FixtureRequest(typeof(string));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<string>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_StringWithoutName_ReturnsEmptyString()
        {
            var request = new FixtureRequest(typeof(string));

            var result = _sut.Resolve(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<string>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_Int_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(int));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_Char_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(char));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Resolve_Object_ReturnsNull()
        {
            var request = new FixtureRequest(typeof(object));

            var result = _sut.Resolve(request, _contextMock.Object);

            Assert.That(result, Is.Null);
        }
    }
}
