using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.FixtureFactories.WithMatching;
using FixtureBuilder.FixtureFactories.WithMatching.WithRules;

namespace FixtureBuilder.FixtureFactories.RootConfigurationBuilders
{
    /// <summary>
    /// Configures options, value providers, converters, type links, and value registrations scoped to fixtures of type <typeparamref name="TRoot"/>. Obtained via <see cref="FixtureFactory.WhenBuilding{TRoot}"/>.
    /// </summary>
    /// <typeparam name="TRoot">The fixture type this configuration applies to.</typeparam>
    public sealed class RootConfigurationBuilder<TRoot> : IConfigurationBuilder<RootConfigurationBuilder<TRoot>>, IProviderBuilder<RootConfigurationBuilder<TRoot>>
    {
        private readonly IFixtureContext _context;

        private readonly MatchingProviderBuilder _innerBuilder = new([new RootTypeRule(typeof(TRoot))]);

        /// <inheritdoc />
        public FixtureOptions Options { set { _context.AddRootOptions(typeof(TRoot), value); } }

        internal RootConfigurationBuilder(IFixtureContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            _context = context;
        }

        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> SetOptions(Action<FixtureOptions> optionsAction)
        {
            var options = _context.GetBaseOptions().Clone();
            optionsAction(options);
            _context.AddRootOptions(typeof(TRoot), options);
            return this;
        }

        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> AddProvider(ICustomProvider provider)
        {
            var adaptedProvider = new CustomProviderAdapter(provider);
            var decoratedRootProvider = new RootProviderDecorator(adaptedProvider, typeof(TRoot));
            _context.ValueProvider.AddProvider(decoratedRootProvider);
            return this;
        }

        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> AddConverter(ICustomConverter converter)
        {
            var adaptedConverter = new CustomConverterAdapter(converter);
            var decoratedRootConverter = new RootConverterDecorator(adaptedConverter, typeof(TRoot));
            _context.Converter.Composite.AddConverter(decoratedRootConverter);
            return this;
        }

        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> AddTypeLink<TIn, TOut>()
            => AddTypeLink(typeof(TIn), typeof(TOut));

        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> AddTypeLink(Type typeIn, Type typeOut)
            => AddTypeLink(new TypeLink(typeIn, typeOut));

        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> AddTypeLink(ICustomTypeLink typeLink)
        {
            var adaptedTypeLink = new CustomTypeLinkAdapter(typeLink);
            return AddTypeLink(adaptedTypeLink);
        }

        private RootConfigurationBuilder<TRoot> AddTypeLink(ITypeLink typeLink)
        {
            var rootTypeLink = new RootTypeLinkDecorator(typeLink, typeof(TRoot));
            _context.TypeLink.AddTypeLink(rootTypeLink);
            return this;
        }

        private RootConfigurationBuilder<TRoot> Add(MatchingProvider provider)
        {
            _context.ValueProvider.AddProvider(provider);
            return this;
        }

        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> With<T>(T value) => Add(_innerBuilder.With(value));
        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> With<T>(Func<T> func) => Add(_innerBuilder.With(func));
        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> With<T>(T value, string name) => Add(_innerBuilder.With(value, name));
        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> With<T>(Func<T> func, string name) => Add(_innerBuilder.With(func, name));

        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> WithParameter<T>(T value) => Add(_innerBuilder.WithParameter(value));
        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> WithParameter<T>(Func<T> func) => Add(_innerBuilder.WithParameter(func));
        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> WithParameter<T>(T value, string name) => Add(_innerBuilder.WithParameter(value, name));
        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> WithParameter<T>(Func<T> func, string name) => Add(_innerBuilder.WithParameter(func, name));

        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> WithPropertyOrField<T>(T value) => Add(_innerBuilder.WithPropertyOrField(value));
        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> WithPropertyOrField<T>(Func<T> func) => Add(_innerBuilder.WithPropertyOrField(func));
        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> WithPropertyOrField<T>(T value, string name) => Add(_innerBuilder.WithPropertyOrField(value, name));
        /// <inheritdoc />
        public RootConfigurationBuilder<TRoot> WithPropertyOrField<T>(Func<T> func, string name) => Add(_innerBuilder.WithPropertyOrField(func, name));
    }
}
