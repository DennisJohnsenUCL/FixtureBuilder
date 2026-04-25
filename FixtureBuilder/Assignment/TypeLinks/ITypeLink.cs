using FixtureBuilder.Core;

namespace FixtureBuilder.Assignment.TypeLinks
{
    public interface ITypeLink
    {
        Type? Link(FixtureRequest request);
    }
}
