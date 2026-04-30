using Bogus;

namespace FixtureBuilder.Bogus
{
    public static class FixtureExtensions
    {
        extension(Fixture)
        {
            public static IBogusFixtureConstructor<T> WithBogus<T>() where T : class
            {
                var faker = new Faker();
                return new BogusFixture<T>(faker);
            }
        }
    }
}
