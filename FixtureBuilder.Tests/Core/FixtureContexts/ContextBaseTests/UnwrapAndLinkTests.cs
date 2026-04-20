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
            _typeLinkMock.Setup(r => r.Link(typeof(string))).Returns((Type?)null);
            var context = CreateContext();

            var result = context.UnwrapAndLink(typeof(string));

            Assert.That(result, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void UnwrapAndLink_NullableValueType_ReturnsUnderlyingType()
        {
            _typeLinkMock.Setup(r => r.Link(typeof(int))).Returns((Type?)null);
            var context = CreateContext();

            var result = context.UnwrapAndLink(typeof(int?));

            Assert.That(result, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void UnwrapAndLink_LinkedType_ReturnsLinkedType()
        {
            _typeLinkMock.Setup(r => r.Link(typeof(IDisposable))).Returns(typeof(MemoryStream));
            var context = CreateContext();

            var result = context.UnwrapAndLink(typeof(IDisposable));

            Assert.That(result, Is.EqualTo(typeof(MemoryStream)));
        }

        [Test]
        public void UnwrapAndLink_NullableValueType_UnwrapsThenLinks()
        {
            _typeLinkMock.Setup(r => r.Link(typeof(int))).Returns(typeof(long));
            var context = CreateContext();

            var result = context.UnwrapAndLink(typeof(int?));

            Assert.That(result, Is.EqualTo(typeof(long)));
        }

        [Test]
        public void UnwrapAndLink_NullableValueType_LinksOnUnwrappedType_NotOnNullable()
        {
            _typeLinkMock.Setup(r => r.Link(typeof(int))).Returns((Type?)null);
            var context = CreateContext();

            context.UnwrapAndLink(typeof(int?));

            _typeLinkMock.Verify(r => r.Link(typeof(int)), Times.Once);
            _typeLinkMock.Verify(r => r.Link(typeof(int?)), Times.Never);
        }
    }
}
