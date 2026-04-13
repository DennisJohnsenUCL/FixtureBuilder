using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters
{
    internal interface IValueConverter
    {
        object? Convert(Type target, object value, IFixtureContext context);
    }
}
