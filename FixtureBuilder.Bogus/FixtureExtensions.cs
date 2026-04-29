namespace FixtureBuilder.Bogus
{
    public static class FixtureExtensions
    {
        extension(Fixture)
        {
            public static IBogusFixtureConstructor<T> WithBogus<T>() where T : class
            {
                return new BogusFixture<T>();
            }
        }
    }
}
