using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters.ConverterBuilders;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Core.FixtureContexts.ContextResolvers
{
    internal class EagerContextResolver : IContextResolver
    {
        public ConverterGraph Converter { get; }
        public ICompositeTypeLink TypeLink { get; }
        public IUninitializedProvider UninitializedProvider { get; }
        public ICompositeValueProvider ValueProvider { get; }
        public IAutoConstructingProvider AutoConstructingProvider { get; }
        public IConstructingProvider ConstructingProvider { get; }

        public EagerContextResolver(ConverterGraph converter,
            ICompositeTypeLink typeLink,
            IUninitializedProvider uninitializedProvider,
            ICompositeValueProvider valueProvider,
            IAutoConstructingProvider autoConstructingProvider,
            IConstructingProvider constructingProvider)
        {
            ArgumentNullException.ThrowIfNull(converter);
            ArgumentNullException.ThrowIfNull(typeLink);
            ArgumentNullException.ThrowIfNull(uninitializedProvider);
            ArgumentNullException.ThrowIfNull(valueProvider);
            ArgumentNullException.ThrowIfNull(autoConstructingProvider);
            ArgumentNullException.ThrowIfNull(constructingProvider);

            Converter = converter;
            TypeLink = typeLink;
            UninitializedProvider = uninitializedProvider;
            ValueProvider = valueProvider;
            AutoConstructingProvider = autoConstructingProvider;
            ConstructingProvider = constructingProvider;
        }
    }
}
