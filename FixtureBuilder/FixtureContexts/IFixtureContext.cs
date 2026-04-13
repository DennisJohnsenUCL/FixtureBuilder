using FixtureBuilder.AutoConstructingProviders;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.ValueConverters;
using FixtureBuilder.ValueProviders;

namespace FixtureBuilder.FixtureContexts
{
    internal interface IFixtureContext :
        IValueConverter,
        IUninitializedProvider,
        IValueProvider,
        IAutoConstructingProvider,
        IOptionsContext
    {
        Type UnwrapAndLink(Type type);
        object? ProvideWithStrategy(FixtureRequest request, InstantiationMethod instantiationMethod, InitializeMembers initializeMembers);
        object InstantiateWithStrategy(FixtureRequest request, InstantiationMethod instantiationMethod, InitializeMembers initializeMembers);
    }
}
