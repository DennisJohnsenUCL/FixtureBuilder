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
            var uninitializedProvider = new Func<IFixtureUninitializedProvider>(() => new UninitializedProviderFactory().CreateDefaultUninitializedProvider());
            var valueProvider = new Func<IValueProvider>(() => new ValueProviderFactory().CreateDefaultValueProvider());
            var resolver = new LazyContextResolver(converter, typeLink, uninitializedProvider, valueProvider);
            var context = new FixtureContext(resolver) as IFixtureContext;
            return context;
        }
    }
}
