namespace FixtureBuilder.FixtureContexts.FixtureContextBuilders
{
    internal interface IFixtureContextFactory
    {
        IFixtureContext CreateLazyContext();
    }
}
