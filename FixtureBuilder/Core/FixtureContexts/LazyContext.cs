using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Core.FixtureContexts
{
    /// <summary>
    /// An <see cref="IContextResolver"/> that defers creation of its components until first access,
    /// using <see cref="Lazy{T}"/> to wrap the provided factory functions.
    /// </summary>
    internal class LazyContext : ContextBase, IFixtureContext
    {
        public override FixtureOptions Options { protected get; set; }

        private readonly Lazy<ConverterGraph> _converter;
        public override ConverterGraph Converter => _converter.Value;

        private readonly Lazy<ICompositeTypeLink> _typeLink;
        public override ICompositeTypeLink TypeLink => _typeLink.Value;

        private readonly Lazy<IUninitializedProvider> _uninitializedProvider;
        public override IUninitializedProvider UninitializedProvider => _uninitializedProvider.Value;

        private readonly Lazy<ICompositeValueProvider> _valueProvider;
        public override ICompositeValueProvider ValueProvider => _valueProvider.Value;

        private readonly Lazy<IAutoConstructingProvider> _autoConstructingProvider;
        public override IAutoConstructingProvider AutoConstructingProvider => _autoConstructingProvider.Value;

        private readonly Lazy<IConstructingProvider> _constructingProvider;
        public override IConstructingProvider ConstructingProvider => _constructingProvider.Value;

        public LazyContext(FixtureOptions options,
            Func<ConverterGraph> converter,
            Func<ICompositeTypeLink> typeLink,
            Func<IUninitializedProvider> uninitializedProvider,
            Func<ICompositeValueProvider> valueProvider,
            Func<IAutoConstructingProvider> autoConstructingProvider,
            Func<IConstructingProvider> constructingProvider)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(converter);
            ArgumentNullException.ThrowIfNull(typeLink);
            ArgumentNullException.ThrowIfNull(uninitializedProvider);
            ArgumentNullException.ThrowIfNull(valueProvider);
            ArgumentNullException.ThrowIfNull(autoConstructingProvider);
            ArgumentNullException.ThrowIfNull(constructingProvider);

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
