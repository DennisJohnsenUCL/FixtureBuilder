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
        IUninitializedProvider,
        IValueProvider,
        IParameterProvider,
        IAutoConstructingProvider
    {
        FixtureOptions Options { get; }
        void SetOptions(FixtureOptions options);
        void SetOptions(Action<FixtureOptions> action);

        object InstantiateWithStrategy(FixtureRequest request, InstantiationMethod instantiationMethod, InitializeMembers initializeMembers);
    }
}
