using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.Casters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters.Casters
{
    internal sealed class ElementConverterTests
    {
        [Test]
        public void Convert_ItemNotAssignable_ReturnsItemConvertedToLinkedTargetType()
        {
            HashSet<int> item = [1, 2, 3];
            List<int> converted = [1, 2, 3];
            var converter = new Mock<ICompositeConverter>();
            converter.Setup(x => x.Convert(new TypeRequest(typeof(List<int>), typeof(object)), item, typeof(IList<int>), It.IsAny<IFixtureContext>())).Returns(converted);
            var context = new Mock<IFixtureContext>();
            context.Setup(x => x.UnwrapAndLink(typeof(IList<int>), typeof(object))).Returns(typeof(List<int>));
            context.Setup(x => x.Converter).Returns(converter.Object);

            var result = ElementConverter.Convert(item, typeof(IList<int>), typeof(object), context.Object);

            Assert.That(result, Is.SameAs(converted));
        }
    }
}
