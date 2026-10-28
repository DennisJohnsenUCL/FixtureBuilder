using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.ContextBaseTests
{
    internal sealed class UnwrapAndLinkTests
    {
        private Mock<ICompositeTypeLink> _typeLinkMock;

        private TestableContext CreateContext()
        {
            return new TestableContext(
                new FixtureOptions(),
                new ConverterGraph(new Mock<IValueConverter>().Object, new Mock<ICompositeConverter>().Object),
                _typeLinkMock.Object,
                new Mock<IUninitializedProvider>().Object,
                new Mock<ICompositeValueProvider>().Object,
                new Mock<IAutoConstructingProvider>().Object,
                new Mock<IConstructingProvider>().Object);
        }

        [SetUp]
        public void SetUp()
        {
            _typeLinkMock = new Mock<ICompositeTypeLink>();
        }

        [Test]
        public void UnwrapAndLink_NonNullableNonLinkedType_ReturnsSameType()
        {
            var request = new FixtureRequest(typeof(string));
            var context = CreateContext();

            var result = context.UnwrapAndLink(request);

            Assert.That(result, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void UnwrapAndLink_NullableValueType_ReturnsUnderlyingType()
        {
            var request = new FixtureRequest(typeof(int?));
            var context = CreateContext();

            var result = context.UnwrapAndLink(request);

            Assert.That(result, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void UnwrapAndLink_LinkedType_ReturnsLinkedType()
        {
            var request = new FixtureRequest(typeof(IDisposable));
            _typeLinkMock.Setup(r => r.Link(It.Is<TypeRequest>(req => req.Type == typeof(IDisposable)))).Returns(typeof(MemoryStream));
            var context = CreateContext();

            var result = context.UnwrapAndLink(request);

            Assert.That(result, Is.EqualTo(typeof(MemoryStream)));
        }

        [Test]
        public void UnwrapAndLink_NullableValueType_LinksOnUnwrappedType_NotOnNullable()
        {
            var request = new FixtureRequest(typeof(int?));
            var context = CreateContext();

            context.UnwrapAndLink(request);

            _typeLinkMock.Verify(r => r.Link(It.Is<TypeRequest>(req => req.Type == typeof(int))), Times.Once);
            _typeLinkMock.Verify(r => r.Link(It.Is<TypeRequest>(req => req.Type == typeof(int?))), Times.Never);
        }

        [Test]
        public void UnwrapAndLink_ByRefType_UnwrapsThenLinks()
        {
            var request = new FixtureRequest(typeof(IDisposable).MakeByRefType());
            _typeLinkMock.Setup(r => r.Link(It.Is<TypeRequest>(req => req.Type == typeof(IDisposable)))).Returns(typeof(MemoryStream));
            var context = CreateContext();

            var result = context.UnwrapAndLink(request);

            Assert.That(result, Is.EqualTo(typeof(MemoryStream)));
        }

        [Test]
        public void UnwrapAndLink_LinksWithRequestRootType()
        {
            var request = new FixtureRequest(typeof(IDisposable), typeof(string));
            var context = CreateContext();

            context.UnwrapAndLink(request);

            _typeLinkMock.Verify(r => r.Link(It.Is<TypeRequest>(req => req.RootType == typeof(string))), Times.Once);
        }

        [Test]
        public void UnwrapAndLink_ByRefLinkedType_DoesNotChangeRequestType()
        {
            var byRefType = typeof(IDisposable).MakeByRefType();
            var request = new FixtureRequest(byRefType);
            _typeLinkMock.Setup(r => r.Link(It.Is<TypeRequest>(req => req.Type == typeof(IDisposable)))).Returns(typeof(MemoryStream));
            var context = CreateContext();

            context.UnwrapAndLink(request);

            Assert.That(request.Type, Is.EqualTo(byRefType));
        }
    }
}
