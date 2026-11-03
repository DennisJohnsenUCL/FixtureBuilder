using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters
{
    internal interface ICompositeConverter
    {
        object? Convert(TypeRequest request, object value, Type originalType, IFixtureContext context);
        void AddConverter(IValueConverter converter);
        void AddRootConverter(IValueConverter converter);
    }
}
