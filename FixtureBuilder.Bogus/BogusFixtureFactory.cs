using Bogus;

namespace FixtureBuilder.Bogus
{
    public class BogusFixtureFactory
    {
        private readonly Faker _faker;
        private readonly FixtureFactory _factory;

        internal BogusFixtureFactory(Faker faker)
        {
            ArgumentNullException.ThrowIfNull(faker);
            _faker = faker;
            _factory = new FixtureFactory();
        }

        public IBogusFixtureConstructor<T> New<T>() where T : class
        {
            return new BogusFixture<T>(_faker, _factory);
        }
    }
}
