using FixtureBuilder.UninitializedProviders.Decorators;
using FixtureBuilder.ValueProviders.Providers;

namespace FixtureBuilder.UninitializedProviders.UninitializedProviderBuilders
{
    internal class UninitializedProviderFactory : IUninitializedProviderFactory
    {
        public IFixtureUninitializedProvider CreateDefaultUninitializedProvider()
        {
            var creator = new RootUninitializedProvider(
                new MemberInitializer(
                    new DataMemberSkipFilter(),
                    new TypeLinkingUninitializedProvider(
                        new CompositeUninitializedProvider(
                            new DefaultBclTypeProvider()))));

            return creator;
        }
    }
}
