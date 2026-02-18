using FixtureBuilder.TypeLinks;
using FixtureBuilder.ValueConverters;

namespace FixtureBuilder.FixtureContexts
{
    internal interface IContextResolver
    {
        IValueConverter GetConverter();
        ITypeLink GetTypeLink();
    }
}
