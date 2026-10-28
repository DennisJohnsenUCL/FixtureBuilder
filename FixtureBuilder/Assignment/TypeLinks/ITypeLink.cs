using FixtureBuilder.Core;

namespace FixtureBuilder.Assignment.TypeLinks
{
    internal interface ITypeLink
    {
        Type? Link(TypeRequest request);
    }
}
