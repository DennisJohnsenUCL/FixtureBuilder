using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters.ConverterBuilders;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Core.FixtureContexts.ContextResolvers
{
    internal interface IContextResolver
    {
        ConverterGraph Converter { get; }
        ICompositeTypeLink TypeLink { get; }
        IUninitializedProvider UninitializedProvider { get; }
        ICompositeValueProvider ValueProvider { get; }
        IAutoConstructingProvider AutoConstructingProvider { get; }
        IConstructingProvider ConstructingProvider { get; }
    }
}
