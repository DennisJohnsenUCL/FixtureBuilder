namespace FixtureBuilder
{
    public interface IFixtureConstructor<T> : IConstructor<IFixtureConfigurator<T>>, IFixtureConfigurator<T> where T : class;
}
