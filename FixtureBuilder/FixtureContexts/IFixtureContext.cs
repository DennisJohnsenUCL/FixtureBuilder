using FixtureBuilder.TypeLinks;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.ValueConverters;

namespace FixtureBuilder.FixtureContexts
{
    internal interface IFixtureContext : IValueConverter, ITypeLink, IFixtureUninitializedProvider;
}
