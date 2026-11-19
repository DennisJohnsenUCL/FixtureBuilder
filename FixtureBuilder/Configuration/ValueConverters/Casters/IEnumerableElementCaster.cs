using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters.Casters
{
    internal interface IEnumerableElementCaster
    {
        object Cast(ConverterRequest request, object value, IFixtureContext context);
    }
}
