using FixtureBuilder.ValueConverters.Decorators;

namespace FixtureBuilder.ValueConverters.ConverterBuilders
{
    internal class ConverterBuilder
    {
        private IValueConverter? _converter;

        public ConverterBuilder() { }

        public IValueConverter Build()
        {
            ValidateCanDecorate();
            return _converter!;
        }

        public ConverterBuilder WithDecorator(IValueConverter converter)
        {
            ArgumentNullException.ThrowIfNull(converter);
            _converter = converter;
            return this;
        }

        public ConverterBuilder WithTypeLinking()
        {
            ValidateCanDecorate();
            _converter = new TypeLinkingConverter(_converter!);
            return this;
        }

        public ConverterBuilder WithDictionaryElementCasting()
        {
            ValidateCanDecorate();
            _converter = new DictionaryElementCastingConverter(_converter!);
            return this;
        }

        public ConverterBuilder WithEnumerableElementCasting()
        {
            ValidateCanDecorate();
            _converter = new EnumerableElementCastingConverter(_converter!);
            return this;
        }

        public ConverterBuilder WithValidation()
        {
            ValidateCanDecorate();
            _converter = new ValidatingConverter(_converter!);
            return this;
        }

        public CompositeConverterBuilder WithStrategies()
        {
            return new CompositeConverterBuilder(this);
        }

        private void ValidateCanDecorate()
        {
            if (_converter == null)
                throw new InvalidOperationException("Converter builder must have an inner Converter before it can be decorated." +
                    " Consider starting with a CompositeConverter.");
        }
    }
}
