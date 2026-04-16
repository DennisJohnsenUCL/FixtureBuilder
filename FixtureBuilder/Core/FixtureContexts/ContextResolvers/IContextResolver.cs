using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Core.FixtureContexts.ContextResolvers
{
    internal interface IContextResolver
    {
        IValueConverter Converter { get; }
        ICompositeTypeLink TypeLink { get; }
        IUninitializedProvider UninitializedProvider { get; }
        ICompositeValueProvider ValueProvider { get; }
        IAutoConstructingProvider AutoConstructingProvider { get; }
    }
}
