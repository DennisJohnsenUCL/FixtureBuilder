using Bogus;
using FixtureBuilder.Bogus.BogusFixtureFactories;
using FixtureBuilder.Bogus.BogusFixtureFactories.Customizations;
using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder.Bogus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Pre-configured Bogus-integrated fixture producer. Where <see cref="FixtureFactory"/> produces consistently configured fixtures,
    /// <see cref="BogusFixtureFactory"/> adds support for <see cref="Faker"/>-based data generation in registrations and produced fixtures.
    /// Use <see cref="FixtureFactoryExtensions"/> to create instances via <c>FixtureFactory.WithBogus()</c>.
    /// </summary>
    public class BogusFixtureFactory : IBogusConfigurationBuilder<BogusFixtureFactory>, IBogusProviderBuilder<BogusFixtureFactory>
    {
        private readonly Faker _faker;
        private readonly FixtureFactory _factory;

        /// <summary>
        /// Gets or sets the randomizer used for data generation. Set this to a seeded <see cref="Randomizer"/> for repeatable results.
        /// </summary>
        public Randomizer Random { get => _faker.Random; set { _faker.Random = value; } }

        /// <summary>
        /// Gets or sets the locale used for data generation (e.g. "en", "de", "fr").
        /// </summary>
        public string Locale { get => _faker.Locale; set { _faker.Locale = value; } }

        internal BogusFixtureFactory(Faker faker)
        {
            ArgumentNullException.ThrowIfNull(faker);
            _faker = faker;
            _factory = new FixtureFactory();
        }

        /// <summary>
        /// Creates a new Bogus-integrated fixture constructor for type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object for which the fixture constructor is created. Must be a reference type.</typeparam>
        /// <returns>An <see cref="IBogusFixtureConstructor{T}"/> for the specified type.</returns>
        public IBogusFixtureConstructor<T> New<T>() where T : class
        {
            return new BogusFixture<T>(_faker, _factory);
        }

        /// <summary>
        /// Creates a new Bogus-integrated fixture configurator for type <typeparamref name="T"/> using an existing instance.
        /// </summary>
        /// <typeparam name="T">The type of the object to configure. Must be a reference type.</typeparam>
        /// <param name="instance">The instance to be used for configuration. Cannot be <see langword="null"/>.</param>
        /// <returns>An <see cref="IBogusFixtureConfigurator{T}"/> for configuring the specified object.</returns>
        public IBogusFixtureConfigurator<T> New<T>(T instance) where T : class
        {
            ArgumentNullException.ThrowIfNull(instance);
            return new BogusFixture<T>(_faker, _factory, instance);
        }

        /// <summary>
        /// Creates and returns an instance of <typeparamref name="T"/> using the <see cref="FixtureOptions.DefaultInstantiationMethod"/> with no additional configuration.
        /// </summary>
        /// <typeparam name="T">The type of the object to build. Must be a reference type.</typeparam>
        /// <returns>The constructed instance of type <typeparamref name="T"/>.</returns>
        public T Build<T>() where T : class
        {
            return ((IBogusFixtureConstructor<T>)new BogusFixture<T>(_faker, _factory)).Build();
        }

        #region Faker configuration methods

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public BogusFixtureFactory With<T>(Func<Faker, T> func)
            => AddToFactory(f => f.With(() => func(_faker)));

        /// <inheritdoc/>
        public BogusFixtureFactory With<T>(Func<Faker, T> func, string name)
            => AddToFactory(f => f.With(() => func(_faker), name));

        /// <inheritdoc/>
        public BogusFixtureFactory WithParameter<T>(Func<Faker, T> func)
            => AddToFactory(f => f.WithParameter(() => func(_faker)));

        /// <inheritdoc/>
        public BogusFixtureFactory WithParameter<T>(Func<Faker, T> func, string name)
            => AddToFactory(f => f.WithParameter(() => func(_faker), name));

        /// <inheritdoc/>
        public BogusFixtureFactory WithPropertyOrField<T>(Func<Faker, T> func)
            => AddToFactory(f => f.WithPropertyOrField(() => func(_faker)));

        /// <inheritdoc/>
        public BogusFixtureFactory WithPropertyOrField<T>(Func<Faker, T> func, string name)
            => AddToFactory(f => f.WithPropertyOrField(() => func(_faker), name));

        #endregion

        #region Passthrough configuration methods

        /// <inheritdoc/>
        public FixtureOptions Options { get => _factory.Options; set { _factory.Options = value; } }

        /// <inheritdoc/>
        public BogusFixtureFactory SetOptions(Action<FixtureOptions> optionsAction)
            => AddToFactory(f => f.SetOptions(optionsAction));

        /// <inheritdoc/>
        public BogusFixtureFactory AddProvider(ICustomProvider provider)
            => AddToFactory(f => f.AddProvider(provider));

        /// <inheritdoc/>
        public BogusFixtureFactory AddConverter(ICustomConverter converter)
            => AddToFactory(f => f.AddConverter(converter));

        /// <inheritdoc/>
        public BogusFixtureFactory AddTypeLink<TIn, TOut>()
            => AddToFactory(f => f.AddTypeLink<TIn, TOut>());

        /// <inheritdoc/>
        public BogusFixtureFactory AddTypeLink(Type typeIn, Type typeOut)
            => AddToFactory(f => f.AddTypeLink(typeIn, typeOut));

        /// <inheritdoc/>
        public BogusFixtureFactory AddTypeLink(ICustomTypeLink typeLink)
            => AddToFactory(f => f.AddTypeLink(typeLink));

        #endregion

        #region Passthrough provider methods

        /// <inheritdoc/>
        public BogusFixtureFactory With<T>(T value)
            => AddToFactory(f => f.With(value));

        /// <inheritdoc/>
        public BogusFixtureFactory With<T>(Func<T> func)
            => AddToFactory(f => f.With(func));

        /// <inheritdoc/>
        public BogusFixtureFactory With<T>(T value, string name)
            => AddToFactory(f => f.With(value, name));

        /// <inheritdoc/>
        public BogusFixtureFactory With<T>(Func<T> func, string name)
            => AddToFactory(f => f.With(func, name));

        /// <inheritdoc/>
        public BogusFixtureFactory WithParameter<T>(T value)
            => AddToFactory(f => f.WithParameter(value));

        /// <inheritdoc/>
        public BogusFixtureFactory WithParameter<T>(Func<T> func)
            => AddToFactory(f => f.WithParameter(func));

        /// <inheritdoc/>
        public BogusFixtureFactory WithParameter<T>(T value, string name)
            => AddToFactory(f => f.WithParameter(value, name));

        /// <inheritdoc/>
        public BogusFixtureFactory WithParameter<T>(Func<T> func, string name)
            => AddToFactory(f => f.WithParameter(func, name));

        /// <inheritdoc/>
        public BogusFixtureFactory WithPropertyOrField<T>(T value)
            => AddToFactory(f => f.WithPropertyOrField(value));

        /// <inheritdoc/>
        public BogusFixtureFactory WithPropertyOrField<T>(Func<T> func)
            => AddToFactory(f => f.WithPropertyOrField(func));

        /// <inheritdoc/>
        public BogusFixtureFactory WithPropertyOrField<T>(T value, string name)
            => AddToFactory(f => f.WithPropertyOrField(value, name));

        /// <inheritdoc/>
        public BogusFixtureFactory WithPropertyOrField<T>(Func<T> func, string name)
            => AddToFactory(f => f.WithPropertyOrField(func, name));

        #endregion

        /// <summary>
        /// Begins a root-scoped configuration block for <typeparamref name="TRoot"/>. Registrations made within the builder action apply only when resolving values for fixtures of type <typeparamref name="TRoot"/>.
        /// </summary>
        public BogusFixtureFactory WhenBuilding<TRoot>(Action<BogusRootConfigurationBuilder<TRoot>> builderAction)
        {
            _factory.WhenBuilding<TRoot>(inner =>
            {
                var bogusBuilder = new BogusRootConfigurationBuilder<TRoot>(inner, _faker);
                builderAction(bogusBuilder);
            });
            return this;
        }
    }
}
