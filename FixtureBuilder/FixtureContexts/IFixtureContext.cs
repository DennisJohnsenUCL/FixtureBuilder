using FixtureBuilder.AutoConstructingProviders;
using FixtureBuilder.ParameterProviders;
using FixtureBuilder.TypeLinks;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.ValueConverters;
using FixtureBuilder.ValueProviders;

namespace FixtureBuilder.FixtureContexts
{
    internal interface IFixtureContext :
        IValueConverter,
        ITypeLink,
        IFixtureUninitializedProvider,
        IValueProvider,
        IParameterProvider,
        IAutoConstructingProvider
    {
        FixtureOptions Options { get; }
        void SetOptions(FixtureOptions options);
        void SetOptions(Action<FixtureOptions> action);
    }
}
