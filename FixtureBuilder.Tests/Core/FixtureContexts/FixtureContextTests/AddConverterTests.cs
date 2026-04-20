using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Core.FixtureContexts.ContextResolvers;
using Moq;

namespace FixtureBuilder.Tests.Core.FixtureContexts.FixtureContextTests
{
    internal sealed class FixtureContextAddConverterTests
    {
        [Test]
        public void AddConverter_DelegatesToResolverConverterComposite()
        {
            var compositeConverter = new Mock<ICompositeConverter>();
            var graph = new ConverterGraph(new Mock<IValueConverter>().Object, compositeConverter.Object);
            var resolver = new Mock<IContextResolver>();
            resolver.Setup(x => x.Converter).Returns(graph);

            var context = new FixtureContext(resolver.Object, FixtureOptions.Default);
            var converter = new Mock<IValueConverter>().Object;

            context.AddConverter(converter);

            compositeConverter.Verify(x => x.AddConverter(converter), Times.Once);
        }
    }
}
