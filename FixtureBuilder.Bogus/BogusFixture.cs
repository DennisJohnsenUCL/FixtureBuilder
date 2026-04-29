namespace FixtureBuilder.Bogus
{
    internal class BogusFixture<T> : IBogusFixtureConstructor<T>, IBogusFixtureConfigurator<T> where T : class
    {
        public BogusFixture() { }

        T IBogusFixtureConfigurator<T>.Build()
        {
            return Fixture.New<T>().Build();
        }
    }
}
