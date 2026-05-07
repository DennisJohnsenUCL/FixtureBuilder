using Bogus;
using FixtureBuilder.Bogus.BogusFixtureFactories;
using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories;
using FixtureBuilder.FixtureFactories.RootConfigurationBuilders;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder.Bogus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class BogusRootConfigurationBuilder<TRoot> : IBogusConfigurationBuilder<BogusRootConfigurationBuilder<TRoot>>, IBogusProviderBuilder<BogusRootConfigurationBuilder<TRoot>>
    {
        private readonly RootConfigurationBuilder<TRoot> _innerBuilder;
        private readonly Faker _faker;

        public BogusRootConfigurationBuilder(RootConfigurationBuilder<TRoot> innerBuilder, Faker faker)
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

        public BogusRootConfigurationBuilder<TRoot> AddBogusProvider(IBogusCustomProvider provider)
        {
            var adaptedProvider = new BogusCustomProviderAdapter(provider, _faker);
            _innerBuilder.AddProvider(adaptedProvider);
            return this;
        }

        #endregion

        #region Faker provider methods

        public BogusRootConfigurationBuilder<TRoot> With<T>(Func<Faker, T> func)
            => AddToBuilder(b => b.With(() => func(_faker)));

        public BogusRootConfigurationBuilder<TRoot> With<T>(Func<Faker, T> func, string name)
            => AddToBuilder(b => b.With(() => func(_faker), name));

        public BogusRootConfigurationBuilder<TRoot> WithParameter<T>(Func<Faker, T> func)
            => AddToBuilder(b => b.WithParameter(() => func(_faker)));

        public BogusRootConfigurationBuilder<TRoot> WithParameter<T>(Func<Faker, T> func, string name)
            => AddToBuilder(b => b.WithParameter(() => func(_faker), name));

        public BogusRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(Func<Faker, T> func)
            => AddToBuilder(b => b.WithPropertyOrField(() => func(_faker)));

        public BogusRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(Func<Faker, T> func, string name)
            => AddToBuilder(b => b.WithPropertyOrField(() => func(_faker), name));

        #endregion

        #region Passthrough configuration methods

        public FixtureOptions Options { set => _innerBuilder.Options = value; }

        public BogusRootConfigurationBuilder<TRoot> SetOptions(Action<FixtureOptions> optionsAction)
            => AddToBuilder(b => b.SetOptions(optionsAction));

        public BogusRootConfigurationBuilder<TRoot> AddProvider(ICustomProvider provider)
            => AddToBuilder(b => b.AddProvider(provider));

        public BogusRootConfigurationBuilder<TRoot> AddConverter(ICustomConverter converter)
            => AddToBuilder(b => b.AddConverter(converter));

        public BogusRootConfigurationBuilder<TRoot> AddTypeLink<TIn, TOut>()
            => AddToBuilder(b => b.AddTypeLink<TIn, TOut>());

        public BogusRootConfigurationBuilder<TRoot> AddTypeLink(Type typeIn, Type typeOut)
            => AddToBuilder(b => b.AddTypeLink(typeIn, typeOut));

        public BogusRootConfigurationBuilder<TRoot> AddTypeLink(ICustomTypeLink typeLink)
            => AddToBuilder(b => b.AddTypeLink(typeLink));

        #endregion

        #region Passthrough provider methods

        public BogusRootConfigurationBuilder<TRoot> With<T>(T value)
            => AddToBuilder(b => b.With(value));

        public BogusRootConfigurationBuilder<TRoot> With<T>(Func<T> func)
            => AddToBuilder(b => b.With(func));

        public BogusRootConfigurationBuilder<TRoot> With<T>(T value, string name)
            => AddToBuilder(b => b.With(value, name));

        public BogusRootConfigurationBuilder<TRoot> With<T>(Func<T> func, string name)
            => AddToBuilder(b => b.With(func, name));

        public BogusRootConfigurationBuilder<TRoot> WithParameter<T>(T value)
            => AddToBuilder(b => b.WithParameter(value));

        public BogusRootConfigurationBuilder<TRoot> WithParameter<T>(Func<T> func)
            => AddToBuilder(b => b.WithParameter(func));

        public BogusRootConfigurationBuilder<TRoot> WithParameter<T>(T value, string name)
            => AddToBuilder(b => b.WithParameter(value, name));

        public BogusRootConfigurationBuilder<TRoot> WithParameter<T>(Func<T> func, string name)
            => AddToBuilder(b => b.WithParameter(func, name));

        public BogusRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(T value)
            => AddToBuilder(b => b.WithPropertyOrField(value));

        public BogusRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(Func<T> func)
            => AddToBuilder(b => b.WithPropertyOrField(func));

        public BogusRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(T value, string name)
            => AddToBuilder(b => b.WithPropertyOrField(value, name));

        public BogusRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(Func<T> func, string name)
            => AddToBuilder(b => b.WithPropertyOrField(func, name));

        #endregion
    }
}
