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
        IValueConverter GetConverter();
        ITypeLink GetTypeLink();
        IFixtureUninitializedProvider GetUninitializedProvider();
        IValueProvider GetValueProvider();
        IParameterProvider GetParameterProvider();
        IAutoConstructingProvider GetAutoConstructingProvider();
    }
}
