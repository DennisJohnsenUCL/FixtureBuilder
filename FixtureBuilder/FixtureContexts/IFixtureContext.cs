using FixtureBuilder.TypeLinks;
using FixtureBuilder.ValueConverters;

namespace FixtureBuilder.FixtureContexts
{
    internal interface IFixtureContext : IValueConverter, ITypeLink;
}
