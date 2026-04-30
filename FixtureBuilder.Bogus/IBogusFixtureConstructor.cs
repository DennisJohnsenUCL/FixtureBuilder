using Bogus;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Bogus
{
    public interface IBogusFixtureConstructor<T> : IBogusFixtureConfigurator<T> where T : class
    {
        #region IBogusFixtureConstructor

        public Randomizer Random { get; set; }
        public string Locale { get; set; }

        IBogusFixtureConfigurator<T> UseConstructor(Func<Faker, object[]> args);

        #endregion

        #region IFixtureConstructor

        IBogusFixtureConfigurator<T> CreateUninitialized();

        IBogusFixtureConfigurator<T> CreateUninitialized(InitializeMembers initializeMembers);

        IBogusFixtureConfigurator<T> UseConstructor(params object[] args);

        IBogusFixtureConfigurator<T> UseAutoConstructor();

        #endregion
    }
}
