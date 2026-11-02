using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters.Decorators
{
    internal interface IEnumerableElementCaster
    {
        object Cast(TypeRequest request, object value, IFixtureContext context);
    }
}
