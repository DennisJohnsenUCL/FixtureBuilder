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
        public void Convert_UnwrapAndLinkReturnsLinkedType_PassesLinkedTypeToConverter()
        {
            var targetType = typeof(IList<int>);
            var linkedType = typeof(List<int>);
            var item = new HashSet<int> { 1, 2, 3 };

            var converter = new Mock<ICompositeConverter>();
            converter.Setup(x => x.Convert(It.Is<TypeRequest>(r => r.Type == linkedType), item, It.IsAny<Type>(), It.IsAny<IFixtureContext>())).Returns(new List<int> { 1, 2, 3 });

            var context = new Mock<IFixtureContext>();
            context.Setup(c => c.Converter).Returns(converter.Object);
            context.Setup(c => c.UnwrapAndLink(targetType, targetType)).Returns(linkedType);

            ElementConverter.Convert(item, targetType, targetType, context.Object);

            converter.Verify(x => x.Convert(It.Is<TypeRequest>(r => r.Type == linkedType), item, targetType, context.Object), Times.Once);
            converter.Verify(x => x.Convert(It.Is<TypeRequest>(r => r.Type == targetType), It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void Convert_UnwrapAndLinkReturnsLinkedType_ReturnsResult()
        {
            var targetType = typeof(IList<int>);
            var linkedType = typeof(List<int>);
            var item = new HashSet<int> { 1, 2, 3 };
            var expectedResult = new List<int> { 1, 2, 3 };

            var converter = new Mock<ICompositeConverter>();
            converter.Setup(x => x.Convert(It.Is<TypeRequest>(r => r.Type == linkedType), item, It.IsAny<Type>(), It.IsAny<IFixtureContext>())).Returns(expectedResult);

            var context = new Mock<IFixtureContext>();
            context.Setup(c => c.Converter).Returns(converter.Object);
            context.Setup(c => c.UnwrapAndLink(targetType, targetType)).Returns(linkedType);

            var result = ElementConverter.Convert(item, targetType, targetType, context.Object);

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
