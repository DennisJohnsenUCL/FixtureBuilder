using FixtureBuilder.Assignment.TypeLinks;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Configuration.ValueConverters;
using FixtureBuilder.Configuration.ValueConverters.ConverterBuilders;
using FixtureBuilder.Creation.AutoConstructingProviders;
using FixtureBuilder.Creation.ConstructingProviders;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Core.FixtureContexts
{
    internal static class FixtureContextFactory
    {
        public static IFixtureContext CreateLazyContext()
        {
            var converter = new Func<ConverterGraph>(() => ConverterFactory.CreateDefaultConverter());
            var typeLink = new Func<ICompositeTypeLink>(() => TypeLinkFactory.CreateDefaultTypeLink());
            var uninitializedProvider = new Func<IUninitializedProvider>(() => UninitializedProviderFactory.CreateDefaultUninitializedProvider());
            var valueProvider = new Func<ICompositeValueProvider>(() => ValueProviderFactory.CreateDefaultValueProvider());
            var autoConstructingProvider = new Func<IAutoConstructingProvider>(() => new AutoConstructingProvider());
            var constructingProvider = new Func<IConstructingProvider>(() => new ConstructingProvider());

            var options = FixtureOptions.Default;
            var context = new LazyContext(options, converter, typeLink, uninitializedProvider, valueProvider, autoConstructingProvider, constructingProvider);
            return context;
        }

        public static IFixtureContext CreateEagerContext(FixtureOptions? options)
        {
            var converter = ConverterFactory.CreateDefaultConverter();
            var typeLink = TypeLinkFactory.CreateDefaultTypeLink();
            var uninitializedProvider = UninitializedProviderFactory.CreateDefaultUninitializedProvider();
            var valueProvider = ValueProviderFactory.CreateDefaultValueProvider();
            var autoConstructingProvider = new AutoConstructingProvider();
            var constructingProvider = new ConstructingProvider();

            options ??= FixtureOptions.Default;
            var context = new EagerContext(options, converter, typeLink, uninitializedProvider, valueProvider, autoConstructingProvider, constructingProvider);
            return context;
        }
    }
}
