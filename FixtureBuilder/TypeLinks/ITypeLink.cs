namespace FixtureBuilder.TypeLinks
{
    public interface ITypeLink
    {
        Type? Link(Type target);
    }
}
