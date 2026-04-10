using FixtureBuilder.AutoConstructingProviders;
using FixtureBuilder.TypeLinks;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.ValueConverters;
using FixtureBuilder.ValueConverters.ConverterBuilders;
using FixtureBuilder.ValueProviders;

namespace FixtureBuilder.FixtureContexts
{
    internal class FixtureContextFactory
    {
        public static IFixtureContext CreateLazyContext()
        {
            var converter = new Func<IValueConverter>(() => ConverterFactory.CreateDefaultConverter());
            var typeLink = new Func<ITypeLink>(() => TypeLinkFactory.CreateDefaultTypeLink());
            var uninitializedProvider = new Func<IUninitializedProvider>(() => UninitializedProviderFactory.CreateDefaultUninitializedProvider());
            var valueProvider = new Func<IValueProvider>(() => ValueProviderFactory.CreateDefaultValueProvider());
            var autoConstructingProvider = new Func<IAutoConstructingProvider>(() => new AutoConstructingProvider());

            var resolver = new LazyContextResolver(converter, typeLink, uninitializedProvider, valueProvider, autoConstructingProvider);
            var options = FixtureOptions.Default;
            var context = new FixtureContext(resolver, options) as IFixtureContext;
            return context;
        }
    }
}
