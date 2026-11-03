using System.Reflection;
using FixtureBuilder.Configuration;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.FieldHelperTests
{
    internal sealed class SetBackingFieldValueTests
    {
        private class Target
        {
            private readonly IList<int> _numbers = null!;
            public IList<int> Numbers => _numbers;
        }

        private static readonly FieldInfo NumbersField = typeof(Target).GetField("_numbers", BindingFlags.Instance | BindingFlags.NonPublic)!;
        private static readonly PropertyInfo NumbersProperty = typeof(Target).GetProperty(nameof(Target.Numbers))!;

        [Test]
        public void SetBackingFieldValue_UnwrapAndLinkReturnsLinkedType_PassesLinkedTypeToConverter()
        {
            var fieldType = typeof(IList<int>);
            var linkedType = typeof(List<int>);
            var value = new HashSet<int> { 1, 2, 3 };

            var converter = new Mock<ICompositeConverter>();
            converter.Setup(x => x.Convert(It.Is<TypeRequest>(r => r.Type == linkedType), value, It.IsAny<Type>(), It.IsAny<IFixtureContext>())).Returns(new List<int> { 1, 2, 3 });

            var context = new Mock<IFixtureContext>();
            context.Setup(c => c.Converter).Returns(converter.Object);
            context.Setup(c => c.UnwrapAndLink(fieldType, typeof(Target))).Returns(linkedType);

            FieldHelper.SetBackingFieldValue(NumbersField, NumbersProperty, new Target(), value, typeof(Target), context.Object);

            converter.Verify(x => x.Convert(It.Is<TypeRequest>(r => r.Type == linkedType), value, fieldType, context.Object), Times.Once);
            converter.Verify(x => x.Convert(It.Is<TypeRequest>(r => r.Type == fieldType), It.IsAny<object>(), It.IsAny<Type>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void SetBackingFieldValue_UnwrapAndLinkReturnsLinkedType_AssignsResult()
        {
            var fieldType = typeof(IList<int>);
            var linkedType = typeof(List<int>);
            var value = new HashSet<int> { 1, 2, 3 };
            var expectedResult = new List<int> { 1, 2, 3 };

            var converter = new Mock<ICompositeConverter>();
            converter.Setup(x => x.Convert(It.Is<TypeRequest>(r => r.Type == linkedType), value, It.IsAny<Type>(), It.IsAny<IFixtureContext>())).Returns(expectedResult);

            var context = new Mock<IFixtureContext>();
            context.Setup(c => c.Converter).Returns(converter.Object);
            context.Setup(c => c.UnwrapAndLink(fieldType, typeof(Target))).Returns(linkedType);

            var target = new Target();
            FieldHelper.SetBackingFieldValue(NumbersField, NumbersProperty, target, value, typeof(Target), context.Object);

            Assert.That(target.Numbers, Is.SameAs(expectedResult));
        }
    }
}
