using FixtureBuilder.AutoConstructingProviders;
using FixtureBuilder.ParameterProviders;
using FixtureBuilder.TypeLinks;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.ValueConverters;
using FixtureBuilder.ValueProviders;

namespace FixtureBuilder.FixtureContexts
{
    internal interface IContextResolver
    {
        IValueConverter Converter { get; }
        ITypeLink TypeLink { get; }
        IUninitializedProvider UninitializedProvider { get; }
        IValueProvider ValueProvider { get; }
        IParameterProvider ParameterProvider { get; }
        IAutoConstructingProvider AutoConstructingProvider { get; }
    }
}
