using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters.Casters
{
    internal interface IDictionaryElementCaster
    {
        object Cast(ConverterRequest request, object value, IFixtureContext context);
    }
}
