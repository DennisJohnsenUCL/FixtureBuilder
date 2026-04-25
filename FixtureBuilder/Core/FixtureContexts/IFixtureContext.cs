using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Core.FixtureContexts
{
    internal interface IFixtureContext : IOptionsContext
    {
        ConverterGraph Converter { get; }
        ICompositeTypeLink TypeLink { get; }
        IUninitializedProvider UninitializedProvider { get; }
        ICompositeValueProvider ValueProvider { get; }
        IAutoConstructingProvider AutoConstructingProvider { get; }
        IConstructingProvider ConstructingProvider { get; }

        Type UnwrapAndLink(FixtureRequest request);
        object? ProvideWithStrategy(FixtureRequest request, InstantiationMethod instantiationMethod, InitializeMembers initializeMembers);
        object InstantiateWithStrategy(FixtureRequest request, InstantiationMethod instantiationMethod, InitializeMembers initializeMembers);
    }
}
