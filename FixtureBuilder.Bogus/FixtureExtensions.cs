using Bogus;

namespace FixtureBuilder.Bogus
{
    /// <summary>
    /// Provides extension methods on <see cref="Fixture"/> for creating Bogus-integrated fixtures.
    /// </summary>
    public static class FixtureExtensions
    {
        extension(Fixture)
        {
            /// <summary>
            /// Creates a new Bogus-integrated fixture constructor for the specified type using the default locale ("en").
            /// </summary>
            /// <typeparam name="T">The type of the object for which the fixture constructor is created. Must be a reference type.</typeparam>
            /// <returns>An instance of <see cref="IBogusFixtureConstructor{T}"/> for the specified type.</returns>
            public static IBogusFixtureConstructor<T> WithBogus<T>() where T : class
            {
                return WithBogus<T>("en");
            }

            /// <summary>
            /// Creates a new Bogus-integrated fixture constructor for the specified type using the given locale.
            /// </summary>
            /// <typeparam name="T">The type of the object for which the fixture constructor is created. Must be a reference type.</typeparam>
            /// <param name="locale">The locale to use for data generation (e.g. "en", "de", "fr").</param>
            /// <returns>An instance of <see cref="IBogusFixtureConstructor{T}"/> for the specified type.</returns>
            public static IBogusFixtureConstructor<T> WithBogus<T>(string locale) where T : class
            {
                var faker = new Faker(locale);
                return new BogusFixture<T>(faker);
            }
        }
    }
}
