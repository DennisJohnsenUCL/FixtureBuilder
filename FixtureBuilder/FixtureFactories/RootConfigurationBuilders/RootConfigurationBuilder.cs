using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories.WithMatching;
using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.FixtureFactories.RootConfigurationBuilders
{
    internal class RootConfigurationBuilder<TRoot> : IRootConfigurationBuilder<TRoot>
    {
        private readonly IFixtureContext _context;

        private readonly MatchingProviderBuilder _innerBuilder = new([new RootTypeRule(typeof(TRoot))]);

        public FixtureOptions Options { set { _context.AddRootOptions(typeof(TRoot), value); } }

        public RootConfigurationBuilder(IFixtureContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            _context = context;
        }

        public void SetOptions(Action<FixtureOptions> optionsAction)
        {
            var options = _context.GetBaseOptions().Clone();
            optionsAction(options);
            _context.AddRootOptions(typeof(TRoot), options);
        }

        public void AddProvider(ICustomProvider provider)
        {
            var adaptedProvider = new CustomProviderAdapter(provider);
            var decoratedRootProvider = new RootProviderDecorator(adaptedProvider, typeof(TRoot));
            _context.ValueProvider.AddProvider(decoratedRootProvider);
        }

        public void AddConverter(ICustomConverter converter)
        {
            var adaptedConverter = new CustomConverterAdapter(converter);
            var decoratedRootConverter = new RootConverterDecorator(adaptedConverter, typeof(TRoot));
            _context.Converter.Composite.AddConverter(decoratedRootConverter);
        }

        public void AddTypeLink<TIn, TOut>()
            => AddTypeLink(typeof(TIn), typeof(TOut));

        public void AddTypeLink(Type typeIn, Type typeOut)
            => AddTypeLink(new TypeLink(typeIn, typeOut));

        public void AddTypeLink(ICustomTypeLink typeLink)
        {
            var adaptedTypeLink = new CustomTypeLinkAdapter(typeLink);
            AddTypeLink(adaptedTypeLink);
        }

        private void AddTypeLink(ITypeLink typeLink)
        {
            var rootTypeLink = new RootTypeLinkDecorator(typeLink, typeof(TRoot));
            _context.TypeLink.AddTypeLink(rootTypeLink);
        }

        private RootConfigurationBuilder<TRoot> Add(MatchingProvider provider)
        {
            _context.ValueProvider.AddProvider(provider);
            return this;
        }

        public IRootConfigurationBuilder<TRoot> With<T>(T value) => Add(_innerBuilder.With(value));
        public IRootConfigurationBuilder<TRoot> With<T>(Func<T> func) => Add(_innerBuilder.With(func));
        public IRootConfigurationBuilder<TRoot> With<T>(T value, string name) => Add(_innerBuilder.With(value, name));
        public IRootConfigurationBuilder<TRoot> With<T>(Func<T> func, string name) => Add(_innerBuilder.With(func, name));

        public IRootConfigurationBuilder<TRoot> WithParameter<T>(T value) => Add(_innerBuilder.WithParameter(value));
        public IRootConfigurationBuilder<TRoot> WithParameter<T>(Func<T> func) => Add(_innerBuilder.WithParameter(func));
        public IRootConfigurationBuilder<TRoot> WithParameter<T>(T value, string name) => Add(_innerBuilder.WithParameter(value, name));
        public IRootConfigurationBuilder<TRoot> WithParameter<T>(Func<T> func, string name) => Add(_innerBuilder.WithParameter(func, name));

        public IRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(T value) => Add(_innerBuilder.WithPropertyOrField(value));
        public IRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(Func<T> func) => Add(_innerBuilder.WithPropertyOrField(func));
        public IRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(T value, string name) => Add(_innerBuilder.WithPropertyOrField(value, name));
        public IRootConfigurationBuilder<TRoot> WithPropertyOrField<T>(Func<T> func, string name) => Add(_innerBuilder.WithPropertyOrField(func, name));
    }
}
