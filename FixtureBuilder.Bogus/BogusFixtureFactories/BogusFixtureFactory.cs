using Bogus;
using FixtureBuilder.Bogus.BogusFixtureFactories;
using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder.Bogus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class BogusFixtureFactory : IBogusConfigurationBuilder, IBogusProviderBuilder<BogusFixtureFactory>
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

        #region Passthrough configuration methods

        public FixtureOptions Options { get => _factory.Options; set { _factory.Options = value; } }

        public void SetOptions(Action<FixtureOptions> optionsAction)
            => _factory.SetOptions(optionsAction);

        public void AddProvider(ICustomProvider provider)
            => _factory.AddProvider(provider);

        public void AddConverter(ICustomConverter converter)
            => _factory.AddConverter(converter);

        public void AddTypeLink<TIn, TOut>()
            => _factory.AddTypeLink<TIn, TOut>();

        public void AddTypeLink(Type typeIn, Type typeOut)
            => _factory.AddTypeLink(typeIn, typeOut);

        public void AddTypeLink(ICustomTypeLink typeLink)
            => _factory.AddTypeLink(typeLink);

        #endregion

        //TODO: WhenBuilding
    }
}
