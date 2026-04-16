using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Core.FixtureContexts.ContextResolvers
{
    /// <summary>
    /// An <see cref="IContextResolver"/> that defers creation of its components until first access,
    /// using <see cref="Lazy{T}"/> to wrap the provided factory functions.
    /// </summary>
    internal class LazyContextResolver : IContextResolver
    {
        private readonly Lazy<IValueConverter> _converter;
        private readonly Lazy<ICompositeTypeLink> _typeLink;
        private readonly Lazy<IUninitializedProvider> _uninitializedProvider;
        private readonly Lazy<ICompositeValueProvider> _valueProvider;
        private readonly Lazy<IAutoConstructingProvider> _autoConstructingProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyContextResolver"/> class.
        /// </summary>
        /// <param name="converter">A factory function that produces the <see cref="IValueConverter"/> on first use.</param>
        /// <param name="typeLink">A factory function that produces the <see cref="ITypeLink"/> on first use.</param>
        /// <param name="uninitializedProvider">A factory function that produces the <see cref="IUninitializedProvider"/> on first use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="converter"/>, <paramref name="typeLink"/>, or <paramref name="uninitializedProvider"/> is <see langword="null"/>.
        /// </exception>
        public LazyContextResolver(Func<IValueConverter> converter,
            Func<ICompositeTypeLink> typeLink,
            Func<IUninitializedProvider> uninitializedProvider,
            Func<ICompositeValueProvider> valueProvider,
            Func<IAutoConstructingProvider> autoConstructingProvider)
        {
            ArgumentNullException.ThrowIfNull(converter);
            ArgumentNullException.ThrowIfNull(typeLink);
            ArgumentNullException.ThrowIfNull(uninitializedProvider);
            ArgumentNullException.ThrowIfNull(valueProvider);
            ArgumentNullException.ThrowIfNull(autoConstructingProvider);

            _converter = new Lazy<IValueConverter>(converter);
            _typeLink = new Lazy<ICompositeTypeLink>(typeLink);
            _uninitializedProvider = new Lazy<IUninitializedProvider>(uninitializedProvider);
            _valueProvider = new Lazy<ICompositeValueProvider>(valueProvider);
            _autoConstructingProvider = new Lazy<IAutoConstructingProvider>(autoConstructingProvider);
        }

        /// <summary>
        /// Returns the <see cref="IValueConverter"/>, creating it on first access via the factory provided at construction.
        /// </summary>
        /// <returns>The lazily resolved <see cref="IValueConverter"/>.</returns>
        public IValueConverter Converter => _converter.Value;

        /// <summary>
        /// Returns the <see cref="ITypeLink"/>, creating it on first access via the factory provided at construction.
        /// </summary>
        /// <returns>The lazily resolved <see cref="ITypeLink"/>.</returns>
        public ICompositeTypeLink TypeLink => _typeLink.Value;

        /// <summary>
        /// Returns the <see cref="IUninitializedProvider"/>, creating it on first access via the factory provided at construction.
        /// </summary>
        /// <returns>The lazily resolved <see cref="IUninitializedProvider"/>
        public IUninitializedProvider UninitializedProvider => _uninitializedProvider.Value;

        /// <summary>
        /// Returns the <see cref="IValueProvider"/>, creating it on first access via the factory provided at construction.
        /// </summary>
        /// <returns>The lazily resolved <see cref="IValueProvider"/>
        public ICompositeValueProvider ValueProvider => _valueProvider.Value;

        public IAutoConstructingProvider AutoConstructingProvider => _autoConstructingProvider.Value;
    }
}
