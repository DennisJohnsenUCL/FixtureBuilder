using FixtureBuilder.AutoConstructingProviders;
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
        IAutoConstructingProvider,
        IOptionsContext
    {
        object? ProvideWithStrategy(FixtureRequest request, InstantiationMethod instantiationMethod, InitializeMembers initializeMembers);
        object InstantiateWithStrategy(FixtureRequest request, InstantiationMethod instantiationMethod, InitializeMembers initializeMembers);
    }
}
