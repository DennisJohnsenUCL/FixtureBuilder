using FixtureBuilder.TypeLinks;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.ValueConverters;

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
        private readonly Lazy<IFixtureUninitializedProvider> _uninitializedProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyContextResolver"/> class.
        /// </summary>
        /// <param name="converter">A factory function that produces the <see cref="IValueConverter"/> on first use.</param>
        /// <param name="typeLink">A factory function that produces the <see cref="ITypeLink"/> on first use.</param>
        /// <param name="uninitializedProvider">A factory function that produces the <see cref="IFixtureUninitializedProvider"/> on first use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="converter"/>, <paramref name="typeLink"/>, or <paramref name="uninitializedProvider"/> is <see langword="null"/>.
        /// </exception>
        public LazyContextResolver(Func<IValueConverter> converter,
            Func<ITypeLink> typeLink,
            Func<IFixtureUninitializedProvider> uninitializedProvider)
        {
            ArgumentNullException.ThrowIfNull(converter);
            ArgumentNullException.ThrowIfNull(typeLink);
            ArgumentNullException.ThrowIfNull(uninitializedProvider);

            _converter = new Lazy<IValueConverter>(converter);
            _typeLink = new Lazy<ITypeLink>(typeLink);
            _uninitializedProvider = new Lazy<IFixtureUninitializedProvider>(uninitializedProvider);
        }

        /// <summary>
        /// Returns the <see cref="IValueConverter"/>, creating it on first access via the factory provided at construction.
        /// </summary>
        /// <returns>The lazily resolved <see cref="IValueConverter"/>.</returns>
        public IValueConverter GetConverter()
            => _converter.Value;

        /// <summary>
        /// Returns the <see cref="ITypeLink"/>, creating it on first access via the factory provided at construction.
        /// </summary>
        /// <returns>The lazily resolved <see cref="ITypeLink"/>.</returns>
        public ITypeLink GetTypeLink()
            => _typeLink.Value;

        /// <summary>
        /// Returns the <see cref="IFixtureUninitializedProvider"/>, creating it on first access via the factory provided at construction.
        /// </summary>
        /// <returns>The lazily resolved <see cref="IFixtureUninitializedProvider"/>
        public IFixtureUninitializedProvider GetUninitializedProvider()
            => _uninitializedProvider.Value;
    }
}
