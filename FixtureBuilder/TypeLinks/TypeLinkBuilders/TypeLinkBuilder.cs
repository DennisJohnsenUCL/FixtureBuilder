namespace FixtureBuilder.TypeLinks.TypeLinkBuilders
{
    internal class TypeLinkBuilder
    {
        private ITypeLink? _typeLink;

        public TypeLinkBuilder() { }

        public CompositeTypeLinkBuilder WithStrategies()
        {
            return new CompositeTypeLinkBuilder(this);
        }

        public TypeLinkBuilder WithValidation()
        {
            ValidateCanDecorate();
            _typeLink = new ValidatingTypeLink(_typeLink!);
            return this;
        }

        public ITypeLink Build()
        {
            ValidateCanDecorate();
            return _typeLink!;
        }

        public TypeLinkBuilder WithDecorator(ITypeLink typeLink)
        {
            ArgumentNullException.ThrowIfNull(typeLink);
            _typeLink = typeLink;
            return this;
        }

        private void ValidateCanDecorate()
        {
            if (_typeLink == null)
                throw new InvalidOperationException("TypeLink builder must have an inner TypeLink before it can be decorated." +
                    " Consider starting with a CompositeTypeLink.");
        }
    }
}
