using Bogus;

namespace FixtureBuilder.Bogus
{
    public class BogusFixtureFactory
    {
        private readonly Faker _faker;
        private readonly FixtureFactory _factory;

        public Randomizer Random { get => _faker.Random; set { _faker.Random = value; } }
        public string Locale { get => _faker.Locale; set { _faker.Locale = value; } }

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

        public T Build<T>() where T : class
        {
            return ((IBogusFixtureConstructor<T>)new BogusFixture<T>(_faker, _factory)).Build();
        }
    }
}
