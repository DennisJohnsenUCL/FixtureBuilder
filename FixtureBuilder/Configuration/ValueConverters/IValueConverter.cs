using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters
{
    internal interface IValueConverter
    {
        object? Convert(FixtureRequest request, object value, IFixtureContext context);
    }
}
