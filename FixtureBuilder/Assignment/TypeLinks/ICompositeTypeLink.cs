namespace FixtureBuilder.Assignment.TypeLinks
{
    internal interface ICompositeTypeLink : ITypeLink
    {
        void AddTypeLink(ITypeLink link);
    }
}
