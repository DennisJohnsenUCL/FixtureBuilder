using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Tests.Core.FixtureContexts.ContextBaseTests
{
    internal class TestableContext(
        FixtureOptions options,
        ConverterGraph converter,
        ICompositeTypeLink typeLink,
        IUninitializedProvider uninitializedProvider,
        ICompositeValueProvider valueProvider,
        IAutoConstructingProvider autoConstructingProvider,
        IConstructingProvider constructingProvider) : ContextBase
    {
        public override FixtureOptions Options { get; set; } = options;
        public override ConverterGraph Converter { get; } = converter;
        public override ICompositeTypeLink TypeLink { get; } = typeLink;
        public override IUninitializedProvider UninitializedProvider { get; } = uninitializedProvider;
        public override ICompositeValueProvider ValueProvider { get; } = valueProvider;
        public override IAutoConstructingProvider AutoConstructingProvider { get; } = autoConstructingProvider;
        public override IConstructingProvider ConstructingProvider { get; } = constructingProvider;
    }
}
