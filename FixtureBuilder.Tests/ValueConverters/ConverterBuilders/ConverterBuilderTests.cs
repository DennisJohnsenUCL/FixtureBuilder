using FixtureBuilder.ValueConverters;
using FixtureBuilder.ValueConverters.ConverterBuilders;
using FixtureBuilder.ValueConverters.Decorators;
using Moq;

namespace FixtureBuilder.Tests.ValueConverters.ConverterBuilders
{
    internal class ConverterBuilderTests
    {
        private ConverterBuilder _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new ConverterBuilder();
        }

        #region Constructor

        [Test]
        public void Constructor_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => new ConverterBuilder());
        }

        #endregion

        #region WithDecorator

        [Test]
        public void WithDecorator_NullConverter_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _sut.WithDecorator(null!));

            Assert.That(ex!.ParamName, Is.EqualTo("converter"));
        }

        [Test]
        public void WithDecorator_ValidConverter_ReturnsSameBuilder()
        {
            var result = _sut.WithDecorator(Mock.Of<IValueConverter>());

            Assert.That(result, Is.SameAs(_sut));
        }

        #endregion

        #region WithStrategies

        [Test]
        public void WithStrategies_ReturnsCompositeConverterBuilder()
        {
            var result = _sut.WithStrategies();

            Assert.That(result, Is.InstanceOf<CompositeConverterBuilder>());
        }

        #endregion

        #region WithDictionaryElementCasting

        [Test]
        public void WithDictionaryElementCasting_NoInnerConverter_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.WithDictionaryElementCasting());
        }

        [Test]
        public void WithDictionaryElementCasting_WithInnerConverter_ReturnsSameBuilder()
        {
            _sut.WithDecorator(Mock.Of<IValueConverter>());

            var result = _sut.WithDictionaryElementCasting();

            Assert.That(result, Is.SameAs(_sut));
        }

        [Test]
        public void WithDictionaryElementCasting_WithInnerConverter_BuildReturnsDictionaryElementCastingConverter()
        {
            _sut.WithDecorator(Mock.Of<IValueConverter>());
            _sut.WithDictionaryElementCasting();

            var result = _sut.Build();

            Assert.That(result, Is.InstanceOf<DictionaryElementCastingConverter>());
        }

        #endregion

        #region WithEnumerableElementCasting

        [Test]
        public void WithEnumerableElementCasting_NoInnerConverter_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.WithEnumerableElementCasting());
        }

        [Test]
        public void WithEnumerableElementCasting_WithInnerConverter_ReturnsSameBuilder()
        {
            _sut.WithDecorator(Mock.Of<IValueConverter>());

            var result = _sut.WithEnumerableElementCasting();

            Assert.That(result, Is.SameAs(_sut));
        }

        [Test]
        public void WithEnumerableElementCasting_WithInnerConverter_BuildReturnsEnumerableElementCastingConverter()
        {
            _sut.WithDecorator(Mock.Of<IValueConverter>());
            _sut.WithEnumerableElementCasting();

            var result = _sut.Build();

            Assert.That(result, Is.InstanceOf<EnumerableElementCastingConverter>());
        }

        #endregion

        #region WithTypeLinking

        [Test]
        public void WithTypeLinking_NoInnerConverter_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.WithTypeLinking());
        }

        [Test]
        public void WithTypeLinking_WithInnerConverter_ReturnsSameBuilder()
        {
            _sut.WithDecorator(Mock.Of<IValueConverter>());

            var result = _sut.WithTypeLinking();

            Assert.That(result, Is.SameAs(_sut));
        }

        [Test]
        public void WithTypeLinking_WithInnerConverter_BuildReturnsTypeLinkingConverter()
        {
            _sut.WithDecorator(Mock.Of<IValueConverter>());
            _sut.WithTypeLinking();

            var result = _sut.Build();

            Assert.That(result, Is.InstanceOf<TypeLinkingConverter>());
        }

        #endregion

        #region WithValidation

        [Test]
        public void WithValidation_NoInnerConverter_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.WithValidation());
        }

        [Test]
        public void WithValidation_WithInnerConverter_ReturnsSameBuilder()
        {
            _sut.WithDecorator(Mock.Of<IValueConverter>());

            var result = _sut.WithValidation();

            Assert.That(result, Is.SameAs(_sut));
        }

        #endregion

        #region Build

        [Test]
        public void Build_NoInnerConverter_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _sut.Build());
        }

        [Test]
        public void Build_AfterWithDecorator_ReturnsProvidedConverter()
        {
            var converter = Mock.Of<IValueConverter>();
            _sut.WithDecorator(converter);

            var result = _sut.Build();

            Assert.That(result, Is.SameAs(converter));
        }

        [Test]
        public void Build_FullFluentChain_ReturnsValidatingConverter()
        {
            var result = _sut
                .WithStrategies()
                    .AddEnumerableStrategies()
                    .AddDictionaryStrategies()
                    .And()
                .WithDictionaryElementCasting()
                .WithEnumerableElementCasting()
                .WithTypeLinking()
                .WithValidation()
                .Build();

            Assert.That(result, Is.InstanceOf<ValidatingConverter>());
        }

        [Test]
        public void Build_DecoratorOrder_OutermostIsLastApplied()
        {
            _sut.WithDecorator(Mock.Of<IValueConverter>());
            _sut.WithDictionaryElementCasting();
            _sut.WithValidation();

            var result = _sut.Build();

            Assert.That(result, Is.InstanceOf<ValidatingConverter>());
        }

        #endregion
    }
}
