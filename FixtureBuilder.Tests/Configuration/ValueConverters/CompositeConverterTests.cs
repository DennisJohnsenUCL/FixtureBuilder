using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.Casters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using Moq;

namespace FixtureBuilder.Tests.Configuration.ValueConverters
{
    internal sealed class CompositeConverterTests
    {
        private interface IFoo { }
        private class LinkedFoo : IFoo { }
        private class OtherFoo : IFoo { }

        private IFixtureContext _context;
        private IEnumerableElementCaster _enumerableCaster;
        private IDictionaryElementCaster _dictionaryCaster;

        [SetUp]
        public void Setup()
        {
            _context = new Mock<IFixtureContext>().Object;

            var enumerableCaster = new Mock<IEnumerableElementCaster>();
            enumerableCaster.Setup(x => x.Cast(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()))
                .Returns((TypeRequest _, object v, IFixtureContext _) => v);
            _enumerableCaster = enumerableCaster.Object;

            var dictionaryCaster = new Mock<IDictionaryElementCaster>();
            dictionaryCaster.Setup(x => x.Cast(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()))
                .Returns((TypeRequest _, object v, IFixtureContext _) => v);
            _dictionaryCaster = dictionaryCaster.Object;
        }

        [Test]
        public void Constructor_Constructs()
        {
            var converters = new[] { Mock.Of<IValueConverter>(), Mock.Of<IValueConverter>() };

            Assert.DoesNotThrow(() => new CompositeConverter(converters, _enumerableCaster, _dictionaryCaster));
        }

        [Test]
        public void Convert_NoConverters_ReturnsNoResult()
        {
            var converters = Enumerable.Empty<IValueConverter>();
            var composite = new CompositeConverter(converters, _enumerableCaster, _dictionaryCaster);
            var targetType = new TypeRequest(typeof(string), typeof(string));
            var value = 5;

            var result = composite.Convert(targetType, value, targetType.Type, _context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_NoConverterReturnsResult_ReturnsNull()
        {
            var converter1 = new Mock<IValueConverter>();
            converter1.Setup(x => x.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>())).Returns(NoResult.Instance);

            var converter2 = new Mock<IValueConverter>();
            converter2.Setup(x => x.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>())).Returns(NoResult.Instance);

            var composite = new CompositeConverter([converter1.Object, converter2.Object], _enumerableCaster, _dictionaryCaster);
            var targetType = new TypeRequest(typeof(string), typeof(string));
            var value = 5;

            var result = composite.Convert(targetType, value, targetType.Type, _context);

            Assert.That(result, Is.TypeOf<NoResult>());
        }

        [Test]
        public void Convert_FirstConverterReturnsResult_ReturnsResult()
        {
            var expectedResult = "converted";
            var targetType = new TypeRequest(typeof(string), typeof(string));
            var value = 5;

            var converter1 = new Mock<IValueConverter>();
            converter1.Setup(x => x.Convert(targetType, value, _context)).Returns(expectedResult);

            var converter2 = new Mock<IValueConverter>();

            var composite = new CompositeConverter([converter1.Object, converter2.Object], _enumerableCaster, _dictionaryCaster);

            var result = composite.Convert(targetType, value, targetType.Type, _context);

            Assert.That(result, Is.EqualTo(expectedResult));
            converter1.Verify(x => x.Convert(targetType, value, _context), Times.Once);
            converter2.Verify(x => x.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void Convert_SecondConverterReturnsResult_ReturnsResult()
        {
            var expectedResult = "converted";
            var targetType = new TypeRequest(typeof(string), typeof(string));
            var value = 5;

            var converter1 = new Mock<IValueConverter>();
            converter1.Setup(x => x.Convert(targetType, value, _context)).Returns(NoResult.Instance);

            var converter2 = new Mock<IValueConverter>();
            converter2.Setup(x => x.Convert(targetType, value, _context)).Returns(expectedResult);

            var composite = new CompositeConverter([converter1.Object, converter2.Object], _enumerableCaster, _dictionaryCaster);

            var result = composite.Convert(targetType, value, targetType.Type, _context);

            Assert.That(result, Is.EqualTo(expectedResult));
            converter1.Verify(x => x.Convert(targetType, value, _context), Times.Once);
            converter2.Verify(x => x.Convert(targetType, value, _context), Times.Once);
        }

        [Test]
        public void Convert_StopsAtFirstNonNoResultResult()
        {
            var firstResult = "first";
            var targetType = new TypeRequest(typeof(string), typeof(string));
            var value = 5;

            var converter1 = new Mock<IValueConverter>();
            converter1.Setup(x => x.Convert(targetType, value, _context)).Returns(NoResult.Instance);

            var converter2 = new Mock<IValueConverter>();
            converter2.Setup(x => x.Convert(targetType, value, _context)).Returns(firstResult);

            var converter3 = new Mock<IValueConverter>();
            converter3.Setup(x => x.Convert(targetType, value, _context)).Returns("should not reach");

            var composite = new CompositeConverter([converter1.Object, converter2.Object, converter3.Object], _enumerableCaster, _dictionaryCaster);

            var result = composite.Convert(targetType, value, targetType.Type, _context);

            Assert.That(result, Is.EqualTo(firstResult));
            converter3.Verify(x => x.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void Convert_ConverterReturnsNullForNullableValueType_ReturnsNull()
        {
            var request = new TypeRequest(typeof(int), typeof(int));

            var converter = new Mock<IValueConverter>();
            converter.Setup(x => x.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>())).Returns((object?)null);

            var composite = new CompositeConverter([converter.Object], _enumerableCaster, _dictionaryCaster);

            var result = composite.Convert(request, "Any string", typeof(int?), _context);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Convert_ConverterReturnsNullForNonNullableValueType_Throws()
        {
            var request = new TypeRequest(typeof(int), typeof(int));

            var converter = new Mock<IValueConverter>();
            converter.Setup(x => x.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>())).Returns((object?)null);

            var composite = new CompositeConverter([converter.Object], _enumerableCaster, _dictionaryCaster);

            Assert.Throws<InvalidCastException>(() => composite.Convert(request, "Any string", request.Type, _context));
        }

        [Test]
        public void Convert_ConverterReturnsWrongType_Throws()
        {
            var request = new TypeRequest(typeof(int), typeof(int));

            var converter = new Mock<IValueConverter>();
            converter.Setup(x => x.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>())).Returns("Test string");

            var composite = new CompositeConverter([converter.Object], _enumerableCaster, _dictionaryCaster);

            Assert.Throws<InvalidCastException>(() => composite.Convert(request, "Any string", request.Type, _context));
        }

        [Test]
        public void Convert_ResultAssignableToOriginalTypeButNotRequestedType_ReturnsResult()
        {
            var request = new TypeRequest(typeof(LinkedFoo), typeof(LinkedFoo));
            var expected = new OtherFoo();

            var converter = new Mock<IValueConverter>();
            converter.Setup(x => x.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>())).Returns(expected);

            var composite = new CompositeConverter([converter.Object], _enumerableCaster, _dictionaryCaster);

            var result = composite.Convert(request, "Any string", typeof(IFoo), _context);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public void Convert_PassesEnumerableCastValueToRootAndOtherConverters()
        {
            var targetType = new TypeRequest(typeof(List<long>), typeof(List<long>));
            var value = new List<int> { 1, 2, 3 };
            var castValue = new List<long> { 1L, 2L, 3L };

            var caster = new Mock<IEnumerableElementCaster>();
            caster.Setup(x => x.Cast(targetType, value, _context)).Returns(castValue);

            var rootConverter = new Mock<IValueConverter>();
            rootConverter.Setup(x => x.Convert(targetType, castValue, _context)).Returns(NoResult.Instance);

            var converter = new Mock<IValueConverter>();
            converter.Setup(x => x.Convert(targetType, castValue, _context)).Returns(castValue);

            var sut = new CompositeConverter([converter.Object], caster.Object, _dictionaryCaster);
            sut.AddRootConverter(rootConverter.Object);

            var result = sut.Convert(targetType, value, targetType.Type, _context);

            Assert.That(result, Is.SameAs(castValue));
            caster.Verify(x => x.Cast(targetType, value, _context), Times.Once);
            rootConverter.Verify(x => x.Convert(targetType, castValue, _context), Times.Once);
            converter.Verify(x => x.Convert(targetType, castValue, _context), Times.Once);
        }

        [Test]
        public void Convert_PassesDictionaryCastValueToRootAndOtherConverters()
        {
            var targetType = new TypeRequest(typeof(Dictionary<string, long>), typeof(Dictionary<string, long>));
            var value = new Dictionary<string, int> { { "one", 1 } };
            var castValue = new Dictionary<string, long> { { "one", 1L } };

            var caster = new Mock<IDictionaryElementCaster>();
            caster.Setup(x => x.Cast(targetType, value, _context)).Returns(castValue);

            var rootConverter = new Mock<IValueConverter>();
            rootConverter.Setup(x => x.Convert(targetType, castValue, _context)).Returns(NoResult.Instance);

            var converter = new Mock<IValueConverter>();
            converter.Setup(x => x.Convert(targetType, castValue, _context)).Returns(castValue);

            var sut = new CompositeConverter([converter.Object], _enumerableCaster, caster.Object);
            sut.AddRootConverter(rootConverter.Object);

            var result = sut.Convert(targetType, value, targetType.Type, _context);

            Assert.That(result, Is.SameAs(castValue));
            caster.Verify(x => x.Cast(targetType, value, _context), Times.Once);
            rootConverter.Verify(x => x.Convert(targetType, castValue, _context), Times.Once);
            converter.Verify(x => x.Convert(targetType, castValue, _context), Times.Once);
        }

        [Test]
        public void AddConverter_AddedConverterIsUsedByConvert()
        {
            var targetType = new TypeRequest(typeof(string), typeof(string));
            var value = 5;
            var expected = "converted";

            var addedConverter = new Mock<IValueConverter>();
            addedConverter.Setup(x => x.Convert(targetType, value, _context)).Returns(expected);

            var sut = new CompositeConverter([], _enumerableCaster, _dictionaryCaster);
            sut.AddConverter(addedConverter.Object);

            var result = sut.Convert(targetType, value, targetType.Type, _context);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void AddConverter_PrependedBeforeExistingConverters()
        {
            var targetType = new TypeRequest(typeof(string), typeof(string));
            var value = 5;
            var addedResult = "from-added";
            var originalResult = "from-original";

            var originalConverter = new Mock<IValueConverter>();
            originalConverter.Setup(x => x.Convert(targetType, value, _context)).Returns(originalResult);

            var addedConverter = new Mock<IValueConverter>();
            addedConverter.Setup(x => x.Convert(targetType, value, _context)).Returns(addedResult);

            var sut = new CompositeConverter([originalConverter.Object], _enumerableCaster, _dictionaryCaster);
            sut.AddConverter(addedConverter.Object);

            var result = sut.Convert(targetType, value, targetType.Type, _context);

            Assert.That(result, Is.EqualTo(addedResult));
            originalConverter.Verify(x => x.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void AddRootConverter_ConvertedBeforeLaterAddedConverters()
        {
            var targetType = new TypeRequest(typeof(string), typeof(string));
            var value = 5;
            var rootResult = "from-root";

            var rootConverter = new Mock<IValueConverter>();
            rootConverter.Setup(x => x.Convert(targetType, value, _context)).Returns(rootResult);

            var addedConverter = new Mock<IValueConverter>();
            addedConverter.Setup(x => x.Convert(targetType, value, _context)).Returns("from-added");

            var sut = new CompositeConverter([], _enumerableCaster, _dictionaryCaster);
            sut.AddRootConverter(rootConverter.Object);
            sut.AddConverter(addedConverter.Object);

            var result = sut.Convert(targetType, value, targetType.Type, _context);

            Assert.That(result, Is.EqualTo(rootResult));
            addedConverter.Verify(x => x.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()), Times.Never);
        }

        [Test]
        public void AddRootConverter_PrependedBeforeEarlierRootConverters()
        {
            var targetType = new TypeRequest(typeof(string), typeof(string));
            var value = 5;
            var laterResult = "from-later-root";

            var earlierRootConverter = new Mock<IValueConverter>();
            earlierRootConverter.Setup(x => x.Convert(targetType, value, _context)).Returns("from-earlier-root");

            var laterRootConverter = new Mock<IValueConverter>();
            laterRootConverter.Setup(x => x.Convert(targetType, value, _context)).Returns(laterResult);

            var sut = new CompositeConverter([], _enumerableCaster, _dictionaryCaster);
            sut.AddRootConverter(earlierRootConverter.Object);
            sut.AddRootConverter(laterRootConverter.Object);

            var result = sut.Convert(targetType, value, targetType.Type, _context);

            Assert.That(result, Is.EqualTo(laterResult));
            earlierRootConverter.Verify(x => x.Convert(It.IsAny<TypeRequest>(), It.IsAny<object>(), It.IsAny<IFixtureContext>()), Times.Never);
        }
    }
}
