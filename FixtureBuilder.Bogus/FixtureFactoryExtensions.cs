using Bogus;

namespace FixtureBuilder.Bogus
{
    /// <summary>
    /// Provides extension methods on <see cref="FixtureFactory"/> for creating Bogus-integrated fixture factories.
    /// </summary>
    public static class FixtureFactoryExtensions
    {
        extension(FixtureFactory)
        {
            /// <summary>
            /// Creates a new Bogus-integrated fixture factory using the default locale ("en").
            /// </summary>
            /// <returns>A <see cref="BogusFixtureFactory"/> instance for producing Bogus-integrated fixtures.</returns>
            public static BogusFixtureFactory WithBogus()
            {
                return WithBogus("en");
            }

            /// <summary>
            /// Creates a new Bogus-integrated fixture factory using the given locale.
            /// </summary>
            /// <param name="locale">The locale to use for data generation (e.g. "en", "de", "fr").</param>
            /// <returns>A <see cref="BogusFixtureFactory"/> instance for producing Bogus-integrated fixtures.</returns>
            public static BogusFixtureFactory WithBogus(string locale)
            {
                var faker = new Faker(locale);
                return new BogusFixtureFactory(faker);
            }
        }
    }
}
