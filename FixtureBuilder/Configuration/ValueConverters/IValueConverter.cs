using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters
{
    internal interface IValueConverter
    {
        object? Convert(TypeRequest request, object value, IFixtureContext context);
    }
}
