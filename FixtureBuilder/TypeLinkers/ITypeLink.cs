namespace FixtureBuilder.TypeLinkers
{
    internal interface ITypeLink
    {
        Type? Link(Type target);
    }
}
