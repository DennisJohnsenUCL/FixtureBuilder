using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.ValueConverters
{
    internal interface IValueConverter
    {
        object? Convert(Type target, object value, IFixtureContext context);
    }
}
