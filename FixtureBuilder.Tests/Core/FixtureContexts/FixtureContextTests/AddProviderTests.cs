using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Core.FixtureContexts.ContextResolvers;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.FixtureContextTests
{
    internal sealed class AddProviderTests
    {
        [Test]
        public void AddProvider_DelegatesToResolverValueProvider()
        {
            var compositeValueProvider = new Mock<ICompositeValueProvider>();
            var resolver = new Mock<IContextResolver>();
            resolver.Setup(x => x.ValueProvider).Returns(compositeValueProvider.Object);

            var context = new FixtureContext(resolver.Object, FixtureOptions.Default);
            var provider = new Mock<IValueProvider>().Object;

            context.AddProvider(provider);

            compositeValueProvider.Verify(x => x.AddProvider(provider), Times.Once);
        }
    }
}
