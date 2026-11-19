using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters
{
    internal interface ICompositeConverter
    {
        object? Convert(ConverterRequest request, object value, IFixtureContext context);
        void AddConverter(IValueConverter converter);
        void AddRootConverter(IValueConverter converter);
    }
}
