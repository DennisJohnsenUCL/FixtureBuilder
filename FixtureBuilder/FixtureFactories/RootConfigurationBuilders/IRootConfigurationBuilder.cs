namespace FixtureBuilder.FixtureFactories.RootConfigurationBuilders
{
    public interface IRootConfigurationBuilder<TRoot> : IConfigurationBuilder<IRootConfigurationBuilder<TRoot>>;
}
