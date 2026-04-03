using FixtureBuilder.FixtureProviders;
using FixtureBuilder.TypeLinks;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.ValueConverters;

namespace FixtureBuilder.FixtureContexts
{
    internal interface IContextResolver
    {
        IValueConverter GetConverter();
        ITypeLink GetTypeLink();
        IFixtureUninitializedProvider GetUninitializedProvider();
        IFixtureProvider GetFixtureProvider();
    }
}
