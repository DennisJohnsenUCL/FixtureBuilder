using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Core.FixtureContexts.ContextResolvers
{
    internal class EagerContextResolver : IContextResolver
    {
        public IValueConverter Converter { get; }
        public ITypeLink TypeLink { get; }
        public IUninitializedProvider UninitializedProvider { get; }
        public IValueProvider ValueProvider { get; }
        public IAutoConstructingProvider AutoConstructingProvider { get; }

        public EagerContextResolver(IValueConverter converter,
            ITypeLink typeLink,
            IUninitializedProvider uninitializedProvider,
            IValueProvider valueProvider,
            IAutoConstructingProvider autoConstructingProvider)
        {
            ArgumentNullException.ThrowIfNull(converter);
            ArgumentNullException.ThrowIfNull(typeLink);
            ArgumentNullException.ThrowIfNull(uninitializedProvider);
            ArgumentNullException.ThrowIfNull(valueProvider);
            ArgumentNullException.ThrowIfNull(autoConstructingProvider);

            Converter = converter;
            TypeLink = typeLink;
            UninitializedProvider = uninitializedProvider;
            ValueProvider = valueProvider;
            AutoConstructingProvider = autoConstructingProvider;
        }
    }
}
