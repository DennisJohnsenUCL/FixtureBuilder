namespace FixtureBuilder.FixtureFactories.RootConfigurationBuilders
{
    /// <summary>
    /// Provides configuration methods scoped to fixtures of type <typeparamref name="TRoot"/>. Registrations made through this builder
    /// apply only when the factory is producing fixtures of type <typeparamref name="TRoot"/>.
    /// </summary>
    /// <typeparam name="TRoot">The fixture type to which this configuration scope applies.</typeparam>
    public interface IRootConfigurationBuilder<TRoot> : IConfigurationBuilder<IRootConfigurationBuilder<TRoot>>;
}
