using Bogus;
using FixtureBuilder.Bogus.BogusFixtureFactories;
using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories;
using FixtureBuilder.FixtureFactories.RootConfigurationBuilders;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder.Bogus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Provides root-scoped configuration for fixtures of type <typeparamref name="TRoot"/>,
    /// with support for <see cref="Faker"/>-based data generation.
    /// Registrations made through this builder apply only when resolving values for fixtures of type <typeparamref name="TRoot"/>.
    /// </summary>
    /// <typeparam name="TRoot">The fixture type to which this configuration is scoped.</typeparam>
    public class BogusRootConfigurationBuilder<TRoot> : IBogusConfigurationBuilder<BogusRootConfigurationBuilder<TRoot>>, IBogusProviderBuilder<BogusRootConfigurationBuilder<TRoot>>
    {
        private readonly RootConfigurationBuilder<TRoot> _innerBuilder;
        private readonly Faker _faker;

        internal BogusRootConfigurationBuilder(RootConfigurationBuilder<TRoot> innerBuilder, Faker faker)
        {
            ArgumentNullException.ThrowIfNull(innerBuilder);
            ArgumentNullException.ThrowIfNull(faker);
            _innerBuilder = innerBuilder;
            _faker = faker;
        }

        private BogusRootConfigurationBuilder<TRoot> AddToBuilder(Action<RootConfigurationBuilder<TRoot>> action)
        {
            action(_innerBuilder);
            return this;
        }

        #region Faker configuration methods

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> AddBogusProvider(IBogusCustomProvider provider)
        {
            var adaptedProvider = new BogusCustomProviderAdapter(provider, _faker);
            _innerBuilder.AddProvider(adaptedProvider);
            return this;
        }

        #endregion

        #region Faker provider methods

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> With<T>(Func<Faker, T> func)
            => AddToBuilder(b => b.With(() => func(_faker)));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> With<T>(Func<Faker, T> func, string name)
            => AddToBuilder(b => b.With(() => func(_faker), name));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> WithParameter<T>(Func<Faker, T> func)
            => AddToBuilder(b => b.WithParameter(() => func(_faker)));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> WithParameter<T>(Func<Faker, T> func, string name)
            => AddToBuilder(b => b.WithParameter(() => func(_faker), name));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(Func<Faker, T> func)
            => AddToBuilder(b => b.WithPropertyOrField(() => func(_faker)));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(Func<Faker, T> func, string name)
            => AddToBuilder(b => b.WithPropertyOrField(() => func(_faker), name));

        #endregion

        #region Passthrough configuration methods

        /// <inheritdoc/>
        public FixtureOptions Options { set => _innerBuilder.Options = value; }

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> SetOptions(Action<FixtureOptions> optionsAction)
            => AddToBuilder(b => b.SetOptions(optionsAction));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> AddProvider(ICustomProvider provider)
            => AddToBuilder(b => b.AddProvider(provider));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> AddConverter(ICustomConverter converter)
            => AddToBuilder(b => b.AddConverter(converter));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> AddTypeLink<TIn, TOut>()
            => AddToBuilder(b => b.AddTypeLink<TIn, TOut>());

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> AddTypeLink(Type typeIn, Type typeOut)
            => AddToBuilder(b => b.AddTypeLink(typeIn, typeOut));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> AddTypeLink(ICustomTypeLink typeLink)
            => AddToBuilder(b => b.AddTypeLink(typeLink));

        #endregion

        #region Passthrough provider methods

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> With<T>(T value)
            => AddToBuilder(b => b.With(value));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> With<T>(Func<T> func)
            => AddToBuilder(b => b.With(func));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> With<T>(T value, string name)
            => AddToBuilder(b => b.With(value, name));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> With<T>(Func<T> func, string name)
            => AddToBuilder(b => b.With(func, name));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> WithParameter<T>(T value)
            => AddToBuilder(b => b.WithParameter(value));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> WithParameter<T>(Func<T> func)
            => AddToBuilder(b => b.WithParameter(func));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> WithParameter<T>(T value, string name)
            => AddToBuilder(b => b.WithParameter(value, name));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> WithParameter<T>(Func<T> func, string name)
            => AddToBuilder(b => b.WithParameter(func, name));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(T value)
            => AddToBuilder(b => b.WithPropertyOrField(value));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(Func<T> func)
            => AddToBuilder(b => b.WithPropertyOrField(func));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(T value, string name)
            => AddToBuilder(b => b.WithPropertyOrField(value, name));

        /// <inheritdoc/>
        public BogusRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(Func<T> func, string name)
            => AddToBuilder(b => b.WithPropertyOrField(func, name));

        #endregion
    }
}
