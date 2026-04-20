using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories;
using FixtureBuilder.FixtureFactories.WithMatching;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class FixtureFactory : IProviderBuilder<FixtureFactory>
    {
        private readonly IFixtureContext _context;
        private readonly MatchingProviderBuilder _providerBuilder;

        public FixtureOptions Options
        {
            get => _context.Options;
            set => _context.Options = value;
        }

        public FixtureFactory()
        {
            _context = InitializeContext();
            _providerBuilder = InitializeProviderBuilder();
        }

        public FixtureFactory(FixtureOptions options)
        {
            _context = InitializeContext(options);
            _providerBuilder = InitializeProviderBuilder();
        }

        public IFixtureConstructor<T> New<T>() where T : class
        {
            return new Fixture<T>(_context);
        }

        public IFixtureConstructor<T> New<T>(T instance) where T : class
        {
            ArgumentNullException.ThrowIfNull(instance);
            return new Fixture<T>(instance, _context);
        }

        public T Build<T>() where T : class
        {
            return ((IFixtureConstructor<T>)new Fixture<T>(_context)).Build();
        }

        public void SetOptions(Action<FixtureOptions> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            _context.SetOptions(action);
        }

        #region Add methods

        public void AddTypeLink<TIn, TOut>()
        {
            AddTypeLink(typeof(TIn), typeof(TOut));
        }

        public void AddTypeLink(Type typeIn, Type typeOut)
        {
            AddTypeLink(new TypeLink(typeIn, typeOut));
        }

        public void AddTypeLink(ITypeLink link)
        {
            _context.TypeLink.AddTypeLink(link);
        }

        public void AddProvider(ICustomProvider provider)
        {
            var adaptedProvider = new CustomProviderAdapter(provider);
            _context.ValueProvider.AddProvider(adaptedProvider);
        }

        public void AddConverter(ICustomConverter converter)
        {
            var adaptedConverter = new CustomConverterAdapter(converter);
            _context.Converter.Composite.AddConverter(adaptedConverter);
        }

        #endregion

        #region With methods

        public FixtureFactory With<T>(T value)
            => AddProvider(b => b.With(value));

        public FixtureFactory With<T>(Func<T> func)
            => AddProvider(b => b.With(func));

        public FixtureFactory With<T>(T value, string name)
            => AddProvider(b => b.With(value, name));

        public FixtureFactory With<T>(Func<T> func, string name)
            => AddProvider(b => b.With(func, name));

        public FixtureFactory WithParameter<T>(T value)
            => AddProvider(b => b.WithParameter(value));

        public FixtureFactory WithParameter<T>(Func<T> func)
            => AddProvider(b => b.WithParameter(func));

        public FixtureFactory WithParameter<T>(T value, string name)
            => AddProvider(b => b.WithParameter(value, name));

        public FixtureFactory WithParameter<T>(Func<T> func, string name)
            => AddProvider(b => b.WithParameter(func, name));

        public FixtureFactory WithPropertyOrField<T>(T value)
            => AddProvider(b => b.WithPropertyOrField(value));

        public FixtureFactory WithPropertyOrField<T>(Func<T> func)
            => AddProvider(b => b.WithPropertyOrField(func));

        public FixtureFactory WithPropertyOrField<T>(T value, string name)
            => AddProvider(b => b.WithPropertyOrField(value, name));

        public FixtureFactory WithPropertyOrField<T>(Func<T> func, string name)
            => AddProvider(b => b.WithPropertyOrField(func, name));

        private FixtureFactory AddProvider(Func<MatchingProviderBuilder, MatchingProvider> build)
        {
            _context.ValueProvider.AddProvider(build(_providerBuilder));
            return this;
        }

        public FixtureFactory WhenBuilding<TRoot>(Action<RootProviderBuilder<TRoot>> builderAction)
        {
            ArgumentNullException.ThrowIfNull(builderAction);
            var builder = new RootProviderBuilder<TRoot>();
            builderAction(builder);
            foreach (var provider in builder.Providers)
            {
                _context.ValueProvider.AddProvider(provider);
            }
            return this;
        }

        #endregion

        private static IFixtureContext InitializeContext(FixtureOptions? options = null)
        {
            return FixtureContextFactory.CreateEagerContext(options);
        }

        private static MatchingProviderBuilder InitializeProviderBuilder()
        {
            return new MatchingProviderBuilder();
        }
    }
}
