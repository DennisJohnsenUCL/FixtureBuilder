using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters.Casters
{
    internal interface IDictionaryElementCaster
    {
        object Cast(TypeRequest request, object value, IFixtureContext context);
    }
}
