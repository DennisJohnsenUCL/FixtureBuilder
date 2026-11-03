using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Core.FixtureContexts
{
    internal class LazyContext : ContextBase, IFixtureContext
    {
        public override FixtureOptions Options { protected get; set; }

        private readonly Lazy<ICompositeConverter> _converter;
        public override ICompositeConverter Converter => _converter.Value;

        private readonly Lazy<ICompositeTypeLink> _typeLink;
        public override ICompositeTypeLink TypeLink => _typeLink.Value;

        private readonly Lazy<IRootUninitializedProvider> _uninitializedProvider;
        public override IRootUninitializedProvider UninitializedProvider => _uninitializedProvider.Value;

        private readonly Lazy<ICompositeValueProvider> _valueProvider;
        public override ICompositeValueProvider ValueProvider => _valueProvider.Value;

        private readonly Lazy<IAutoConstructingProvider> _autoConstructingProvider;
        public override IAutoConstructingProvider AutoConstructingProvider => _autoConstructingProvider.Value;

        private readonly Lazy<IConstructingProvider> _constructingProvider;
        public override IConstructingProvider ConstructingProvider => _constructingProvider.Value;

        public LazyContext(FixtureOptions options,
            Func<ICompositeConverter> converter,
            Func<ICompositeTypeLink> typeLink,
            Func<IRootUninitializedProvider> uninitializedProvider,
            Func<ICompositeValueProvider> valueProvider,
            Func<IAutoConstructingProvider> autoConstructingProvider,
            Func<IConstructingProvider> constructingProvider)
        {
            Options = options;
            _converter = new(converter);
            _typeLink = new(typeLink);
            _uninitializedProvider = new(uninitializedProvider);
            _valueProvider = new(valueProvider);
            _autoConstructingProvider = new(autoConstructingProvider);
            _constructingProvider = new(constructingProvider);
        }
    }
}
