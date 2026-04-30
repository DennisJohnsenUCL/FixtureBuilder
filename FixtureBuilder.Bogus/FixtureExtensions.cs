using Bogus;

namespace FixtureBuilder.Bogus
{
    public static class FixtureExtensions
    {
        extension(Fixture)
        {
            public static IBogusFixtureConstructor<T> WithBogus<T>() where T : class
            {
                return WithBogus<T>("en");
            }

            public static IBogusFixtureConstructor<T> WithBogus<T>(string locale) where T : class
            {
                var faker = new Faker(locale);
                return new BogusFixture<T>(faker);
            }
        }
    }
}
