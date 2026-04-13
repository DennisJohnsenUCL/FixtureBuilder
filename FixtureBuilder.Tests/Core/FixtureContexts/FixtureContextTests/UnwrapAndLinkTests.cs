using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.FixtureContextTests
{
    internal sealed class UnwrapAndLinkTests
    {
        [Test]
        public void UnwrapAndLink_NonNullableNonLinkedType_ReturnsSameType()
        {
            var resolverMock = new Mock<IContextResolver>();
            resolverMock.Setup(r => r.TypeLink.Link(typeof(string))).Returns((Type?)null);
            var context = new FixtureContext(resolverMock.Object, new FixtureOptions());

            var result = context.UnwrapAndLink(typeof(string));

            Assert.That(result, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void UnwrapAndLink_NullableValueType_ReturnsUnderlyingType()
        {
            var resolverMock = new Mock<IContextResolver>();
            resolverMock.Setup(r => r.TypeLink.Link(typeof(int))).Returns((Type?)null);
            var context = new FixtureContext(resolverMock.Object, new FixtureOptions());

            var result = context.UnwrapAndLink(typeof(int?));

            Assert.That(result, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void UnwrapAndLink_LinkedType_ReturnsLinkedType()
        {
            var resolverMock = new Mock<IContextResolver>();
            resolverMock.Setup(r => r.TypeLink.Link(typeof(IDisposable))).Returns(typeof(MemoryStream));
            var context = new FixtureContext(resolverMock.Object, new FixtureOptions());

            var result = context.UnwrapAndLink(typeof(IDisposable));

            Assert.That(result, Is.EqualTo(typeof(MemoryStream)));
        }

        [Test]
        public void UnwrapAndLink_NullableValueType_UnwrapsThenLinks()
        {
            var resolverMock = new Mock<IContextResolver>();
            resolverMock.Setup(r => r.TypeLink.Link(typeof(int))).Returns(typeof(long));
            var context = new FixtureContext(resolverMock.Object, new FixtureOptions());

            var result = context.UnwrapAndLink(typeof(int?));

            Assert.That(result, Is.EqualTo(typeof(long)));
        }

        [Test]
        public void UnwrapAndLink_NullableValueType_LinksOnUnwrappedType_NotOnNullable()
        {
            var resolverMock = new Mock<IContextResolver>();
            resolverMock.Setup(r => r.TypeLink.Link(typeof(int))).Returns((Type?)null);
            var context = new FixtureContext(resolverMock.Object, new FixtureOptions());

            context.UnwrapAndLink(typeof(int?));

            resolverMock.Verify(r => r.TypeLink.Link(typeof(int)), Times.Once);
            resolverMock.Verify(r => r.TypeLink.Link(typeof(int?)), Times.Never);
        }
    }
}
