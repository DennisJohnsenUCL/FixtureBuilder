using FixtureBuilder.AutoConstructingProviders;
using FixtureBuilder.ParameterProviders;
using FixtureBuilder.TypeLinks;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.ValueConverters;
using FixtureBuilder.ValueProviders;

namespace FixtureBuilder.FixtureContexts
{
    /// <summary>
    /// An <see cref="IContextResolver"/> that defers creation of its components until first access,
    /// using <see cref="Lazy{T}"/> to wrap the provided factory functions.
    /// </summary>
    internal class LazyContextResolver : IContextResolver
    {
        private readonly Lazy<IValueConverter> _converter;
        private readonly Lazy<ITypeLink> _typeLink;
        private readonly Lazy<IUninitializedProvider> _uninitializedProvider;
        private readonly Lazy<IValueProvider> _valueProvider;
        private readonly Lazy<IParameterProvider> _parameterProvider;
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
            Func<ITypeLink> typeLink,
            Func<IUninitializedProvider> uninitializedProvider,
            Func<IValueProvider> valueProvider,
            Func<IParameterProvider> parameterProvider,
            Func<IAutoConstructingProvider> autoConstructingProvider)
        {
            ArgumentNullException.ThrowIfNull(converter);
            ArgumentNullException.ThrowIfNull(typeLink);
            ArgumentNullException.ThrowIfNull(uninitializedProvider);
            ArgumentNullException.ThrowIfNull(valueProvider);
            ArgumentNullException.ThrowIfNull(parameterProvider);
            ArgumentNullException.ThrowIfNull(autoConstructingProvider);

            _converter = new Lazy<IValueConverter>(converter);
            _typeLink = new Lazy<ITypeLink>(typeLink);
            _uninitializedProvider = new Lazy<IUninitializedProvider>(uninitializedProvider);
            _valueProvider = new Lazy<IValueProvider>(valueProvider);
            _parameterProvider = new Lazy<IParameterProvider>(parameterProvider);
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
        public ITypeLink TypeLink => _typeLink.Value;

        /// <summary>
        /// Returns the <see cref="IUninitializedProvider"/>, creating it on first access via the factory provided at construction.
        /// </summary>
        /// <returns>The lazily resolved <see cref="IUninitializedProvider"/>
        public IUninitializedProvider UninitializedProvider => _uninitializedProvider.Value;

        /// <summary>
        /// Returns the <see cref="IValueProvider"/>, creating it on first access via the factory provided at construction.
        /// </summary>
        /// <returns>The lazily resolved <see cref="IValueProvider"/>
        public IValueProvider ValueProvider => _valueProvider.Value;

        public IParameterProvider ParameterProvider => _parameterProvider.Value;

        public IAutoConstructingProvider AutoConstructingProvider => _autoConstructingProvider.Value;
    }
}
