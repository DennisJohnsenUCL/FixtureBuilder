using Bogus;

namespace FixtureBuilder.Bogus
{
    public static class FixtureFactoryExtensions
    {
        extension(FixtureFactory)
        {
            public static BogusFixtureFactory WithBogus()
            {
                return WithBogus("en");
            }

            public static BogusFixtureFactory WithBogus(string locale)
            {
                var faker = new Faker(locale);
                return new BogusFixtureFactory(faker);
            }
        }
    }
}
