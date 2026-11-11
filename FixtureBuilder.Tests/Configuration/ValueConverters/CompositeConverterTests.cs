using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.Casters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters
{
    internal sealed class CompositeConverterTests
    {
        private class FixedValueConverter(object? result) : IValueConverter
        {
            public object? Convert(TypeRequest request, object value, IFixtureContext context) => result;
        }

        private class NoResultConverter : IValueConverter
        {
            public object? Convert(TypeRequest request, object value, IFixtureContext context) => NoResult.Instance;
        }

        // The casters run before the converters; here they hand the value on unchanged.
        private static CompositeConverter MakeComposite(params IValueConverter[] converters)
        {
            var enumerableCaster = new Mock<IEnumerableElementCaster>();
            enumerableCaster.Setup(x => x.Cast(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()))
                .Returns((TypeRequest _, object value, IFixtureContext _) => value);
            var dictionaryCaster = new Mock<IDictionaryElementCaster>();
            dictionaryCaster.Setup(x => x.Cast(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()))
                .Returns((TypeRequest _, object value, IFixtureContext _) => value);

            return new CompositeConverter(converters, enumerableCaster.Object, dictionaryCaster.Object);
        }

        [Test]
        public void Convert_NoConverterResolves_ReturnsNoResult()
        {
            var composite = MakeComposite(new NoResultConverter(), new NoResultConverter());
            var request = new TypeRequest(typeof(string), typeof(object));

            var result = composite.Convert(request, 5, typeof(string), Mock.Of<IFixtureContext>());

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        // Later converters can be user converters with side effects, so they must not run once a value is converted.
        [Test]
        public void Convert_FirstConverterResolves_DoesNotCallLaterConverters()
        {
            var laterConverter = new Mock<IValueConverter>();
            var composite = MakeComposite(new FixedValueConverter("first"), laterConverter.Object);
            var request = new TypeRequest(typeof(string), typeof(object));

            composite.Convert(request, 5, typeof(string), Mock.Of<IFixtureContext>());

            laterConverter.Verify(x => x.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void Convert_NullForNullableValueTypeTarget_ReturnsNull()
        {
            var composite = MakeComposite(new FixedValueConverter(null));
            var request = new TypeRequest(typeof(int), typeof(object));

            var result = composite.Convert(request, "value", typeof(int?), Mock.Of<IFixtureContext>());

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_ResultAssignableToOriginalTypeButNotLinkedType_ReturnsResult()
        {
            var expected = new OtherMarkerImplementationClass();
            var composite = MakeComposite(new FixedValueConverter(expected));
            var request = new TypeRequest(typeof(MarkerImplementationClass), typeof(object));

            var result = composite.Convert(request, "value", typeof(IMarkerInterface), Mock.Of<IFixtureContext>());

            Assert.That(result, Is.SameAs(expected));
        }
    }
}
