using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters
{
    internal interface IValueConverter
    {
        object? Convert(ConverterRequest request, object value, IFixtureContext context);
    }
}
