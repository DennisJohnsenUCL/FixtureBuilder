using Bogus;
using FixtureBuilder.Bogus.BogusFixtureFactories;
using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder.Bogus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class BogusFixtureFactory : IBogusConfigurationBuilder<BogusFixtureFactory>, IBogusProviderBuilder<BogusFixtureFactory>
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

        #region Faker configuration methods

        public BogusFixtureFactory AddBogusProvider(IBogusCustomProvider provider)
        {
            var adaptedProvider = new BogusCustomProviderAdapter(provider, _faker);
            _factory.AddProvider(adaptedProvider);
            return this;
        }

        #endregion

        private BogusFixtureFactory AddToFactory(Action<FixtureFactory> action)
        {
            action(_factory);
            return this;
        }

        #region Faker provider methods

        public BogusFixtureFactory With<T>(Func<Faker, T> func)
            => AddToFactory(f => f.With(() => func(_faker)));

        public BogusFixtureFactory With<T>(Func<Faker, T> func, string name)
            => AddToFactory(f => f.With(() => func(_faker), name));

        public BogusFixtureFactory WithParameter<T>(Func<Faker, T> func)
            => AddToFactory(f => f.WithParameter(() => func(_faker)));

        public BogusFixtureFactory WithParameter<T>(Func<Faker, T> func, string name)
            => AddToFactory(f => f.WithParameter(() => func(_faker), name));

        public BogusFixtureFactory WithPropertyOrField<T>(Func<Faker, T> func)
            => AddToFactory(f => f.WithPropertyOrField(() => func(_faker)));

        public BogusFixtureFactory WithPropertyOrField<T>(Func<Faker, T> func, string name)
            => AddToFactory(f => f.WithPropertyOrField(() => func(_faker), name));

        #endregion

        #region Passthrough configuration methods

        public FixtureOptions Options { get => _factory.Options; set { _factory.Options = value; } }

        public BogusFixtureFactory SetOptions(Action<FixtureOptions> optionsAction)
            => AddToFactory(f => f.SetOptions(optionsAction));

        public BogusFixtureFactory AddProvider(ICustomProvider provider)
            => AddToFactory(f => f.AddProvider(provider));

        public BogusFixtureFactory AddConverter(ICustomConverter converter)
            => AddToFactory(f => f.AddConverter(converter));

        public BogusFixtureFactory AddTypeLink<TIn, TOut>()
            => AddToFactory(f => f.AddTypeLink<TIn, TOut>());

        public BogusFixtureFactory AddTypeLink(Type typeIn, Type typeOut)
            => AddToFactory(f => f.AddTypeLink(typeIn, typeOut));

        public BogusFixtureFactory AddTypeLink(ICustomTypeLink typeLink)
            => AddToFactory(f => f.AddTypeLink(typeLink));

        #endregion

        #region Passthrough provider methods

        public BogusFixtureFactory With<T>(T value)
            => AddToFactory(f => f.With(value));

        public BogusFixtureFactory With<T>(Func<T> func)
            => AddToFactory(f => f.With(func));

        public BogusFixtureFactory With<T>(T value, string name)
            => AddToFactory(f => f.With(value, name));

        public BogusFixtureFactory With<T>(Func<T> func, string name)
            => AddToFactory(f => f.With(func, name));

        public BogusFixtureFactory WithParameter<T>(T value)
            => AddToFactory(f => f.WithParameter(value));

        public BogusFixtureFactory WithParameter<T>(Func<T> func)
            => AddToFactory(f => f.WithParameter(func));

        public BogusFixtureFactory WithParameter<T>(T value, string name)
            => AddToFactory(f => f.WithParameter(value, name));

        public BogusFixtureFactory WithParameter<T>(Func<T> func, string name)
            => AddToFactory(f => f.WithParameter(func, name));

        public BogusFixtureFactory WithPropertyOrField<T>(T value)
            => AddToFactory(f => f.WithPropertyOrField(value));

        public BogusFixtureFactory WithPropertyOrField<T>(Func<T> func)
            => AddToFactory(f => f.WithPropertyOrField(func));

        public BogusFixtureFactory WithPropertyOrField<T>(T value, string name)
            => AddToFactory(f => f.WithPropertyOrField(value, name));

        public BogusFixtureFactory WithPropertyOrField<T>(Func<T> func, string name)
            => AddToFactory(f => f.WithPropertyOrField(func, name));

        #endregion

        //TODO: WhenBuilding
    }
}
