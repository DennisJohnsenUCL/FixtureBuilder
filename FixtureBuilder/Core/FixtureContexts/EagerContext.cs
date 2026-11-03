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
        public override FixtureOptions Options { protected get; set; }

        public override ICompositeConverter Converter { get; }
        public override ICompositeTypeLink TypeLink { get; }
        public override IRootUninitializedProvider UninitializedProvider { get; }
        public override ICompositeValueProvider ValueProvider { get; }
        public override IAutoConstructingProvider AutoConstructingProvider { get; }
        public override IConstructingProvider ConstructingProvider { get; }

        public EagerContext(FixtureOptions options,
            ICompositeConverter converter,
            ICompositeTypeLink typeLink,
            IRootUninitializedProvider uninitializedProvider,
            ICompositeValueProvider valueProvider,
            IAutoConstructingProvider autoConstructingProvider,
            IConstructingProvider constructingProvider)
        {
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
