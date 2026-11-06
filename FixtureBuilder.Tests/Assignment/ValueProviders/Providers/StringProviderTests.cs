using FixtureBuilder.Assignment.ValueProviders.Providers;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Assignment.ValueProviders.Providers
{
    internal sealed class StringProviderTests
    {
        private StringProvider _sut;
        private Mock<IFixtureContext> _contextMock;

        private class Person
        {
            public string FirstName { get; set; } = "";
        }

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
            var property = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var request = new FixtureRequest(property.PropertyType, property, typeof(object));

            var result = _sut.ResolveValue(request, _contextMock.Object);

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
            var request = new FixtureRequest(typeof(string), TestHelper.UnnamedParameter(typeof(string)), typeof(object));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<string>());
                Assert.That(result, Is.Empty);
            }
        }

        [Test]
        public void Resolve_Int_ReturnsNoResult()
        {
            var request = TestHelper.MakeRequest(typeof(int));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Resolve_Char_ReturnsNoResult()
        {
            var request = TestHelper.MakeRequest(typeof(char));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Resolve_Object_ReturnsNoResult()
        {
            var request = TestHelper.MakeRequest(typeof(object));

            var result = _sut.ResolveValue(request, _contextMock.Object);

            Assert.That(result, Is.TypeOf<NoResult>());
        }
    }
}
