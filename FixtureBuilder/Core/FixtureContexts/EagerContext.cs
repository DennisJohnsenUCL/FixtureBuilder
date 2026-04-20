using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Core.FixtureContexts
{
    internal class EagerContext : ContextBase, IFixtureContext
    {
        public override FixtureOptions Options { get; set; }

        public override ConverterGraph Converter { get; }
        public override ICompositeTypeLink TypeLink { get; }
        public override IUninitializedProvider UninitializedProvider { get; }
        public override ICompositeValueProvider ValueProvider { get; }
        public override IAutoConstructingProvider AutoConstructingProvider { get; }
        public override IConstructingProvider ConstructingProvider { get; }

        public EagerContext(FixtureOptions options,
            ConverterGraph converter,
            ICompositeTypeLink typeLink,
            IUninitializedProvider uninitializedProvider,
            ICompositeValueProvider valueProvider,
            IAutoConstructingProvider autoConstructingProvider,
            IConstructingProvider constructingProvider)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(converter);
            ArgumentNullException.ThrowIfNull(typeLink);
            ArgumentNullException.ThrowIfNull(uninitializedProvider);
            ArgumentNullException.ThrowIfNull(valueProvider);
            ArgumentNullException.ThrowIfNull(autoConstructingProvider);
            ArgumentNullException.ThrowIfNull(constructingProvider);

            Options = options;
            Converter = converter;
            TypeLink = typeLink;
            UninitializedProvider = uninitializedProvider;
            ValueProvider = valueProvider;
            AutoConstructingProvider = autoConstructingProvider;
            ConstructingProvider = constructingProvider;
        }
    }
}
