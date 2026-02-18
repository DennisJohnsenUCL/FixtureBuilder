namespace FixtureBuilder.TypeLinks
{
    internal interface ITypeLink
    {
        Type? Link(Type target);
    }
}
