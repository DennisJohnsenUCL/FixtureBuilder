using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Core.FixtureContexts.ContextResolvers;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.FixtureContextTests
{
    internal sealed class AddTypeLinkTests
    {
        [Test]
        public void AddTypeLink_DelegatesToResolverTypeLink()
        {
            var compositeTypeLink = new Mock<ICompositeTypeLink>();
            var resolver = new Mock<IContextResolver>();
            resolver.Setup(x => x.TypeLink).Returns(compositeTypeLink.Object);

            var context = new FixtureContext(resolver.Object, FixtureOptions.Default);
            var link = new Mock<ITypeLink>().Object;

            context.AddTypeLink(link);

            compositeTypeLink.Verify(x => x.AddTypeLink(link), Times.Once);
        }
    }
}
