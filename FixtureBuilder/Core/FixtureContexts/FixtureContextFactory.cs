using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.ConverterBuilders;
using FixtureBuilder.Core.FixtureContexts.ContextResolvers;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Core.FixtureContexts
{
    internal static class FixtureContextFactory
    {
        public static IFixtureContext CreateLazyContext()
        {
            var converter = new Func<IValueConverter>(() => ConverterFactory.CreateDefaultConverter());
            var typeLink = new Func<ICompositeTypeLink>(() => TypeLinkFactory.CreateDefaultTypeLink());
            var uninitializedProvider = new Func<IUninitializedProvider>(() => UninitializedProviderFactory.CreateDefaultUninitializedProvider());
            var valueProvider = new Func<ICompositeValueProvider>(() => ValueProviderFactory.CreateDefaultValueProvider());
            var autoConstructingProvider = new Func<IAutoConstructingProvider>(() => new AutoConstructingProvider());

            var resolver = new LazyContextResolver(converter, typeLink, uninitializedProvider, valueProvider, autoConstructingProvider);
            var options = FixtureOptions.Default;
            var context = new FixtureContext(resolver, options) as IFixtureContext;
            return context;
        }

        public static IFixtureContext CreateEagerContext(FixtureOptions? options)
        {
            var converter = ConverterFactory.CreateDefaultConverter();
            var typeLink = TypeLinkFactory.CreateDefaultTypeLink();
            var uninitializedProvider = UninitializedProviderFactory.CreateDefaultUninitializedProvider();
            var valueProvider = ValueProviderFactory.CreateDefaultValueProvider();
            var autoConstructingProvider = new AutoConstructingProvider();

            var resolver = new EagerContextResolver(converter, typeLink, uninitializedProvider, valueProvider, autoConstructingProvider);
            options ??= FixtureOptions.Default;
            var context = new FixtureContext(resolver, options) as IFixtureContext;
            return context;
        }
    }
}
