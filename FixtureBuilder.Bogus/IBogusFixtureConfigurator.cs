namespace FixtureBuilder.Bogus
{
    public interface IBogusFixtureConfigurator<T> where T : class
    {
        T Build();
    }
}
