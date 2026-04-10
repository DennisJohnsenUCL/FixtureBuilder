using FixtureBuilder.AutoConstructingProviders;
using FixtureBuilder.ParameterProviders;
using FixtureBuilder.TypeLinks;
using FixtureBuilder.TypeLinks.TypeLinkBuilders;
using FixtureBuilder.UninitializedProviders;
using FixtureBuilder.UninitializedProviders.UninitializedProviderBuilders;
using FixtureBuilder.ValueConverters;
using FixtureBuilder.ValueConverters.ConverterBuilders;
using FixtureBuilder.ValueProviders;
using FixtureBuilder.ValueProviders.ValueProviderBuilders;

namespace FixtureBuilder.FixtureContexts.FixtureContextBuilders
{
    internal class FixtureContextFactory : IFixtureContextFactory
    {
        public IFixtureContext CreateLazyContext()
        {
            var converter = new Func<IValueConverter>(() => new ConverterFactory().CreateDefaultConverter());
            var typeLink = new Func<ITypeLink>(() => new TypeLinkFactory().CreateDefaultTypeLink());
            var uninitializedProvider = new Func<IUninitializedProvider>(() => new UninitializedProviderFactory().CreateDefaultUninitializedProvider());
            var valueProvider = new Func<IValueProvider>(() => new ValueProviderFactory().CreateDefaultValueProvider());
            var parameterProvider = new Func<IParameterProvider>(() => new AutoParameterProvider());
            var autoConstructingProvider = new Func<IAutoConstructingProvider>(() => new AutoConstructingProvider());

            var resolver = new LazyContextResolver(converter, typeLink, uninitializedProvider, valueProvider, parameterProvider, autoConstructingProvider);
            var options = FixtureOptions.Default;
            var context = new FixtureContext(resolver, options) as IFixtureContext;
            return context;
        }
    }
}
