using FixtureBuilder.FixtureProviders.Providers;
using FixtureBuilder.UninitializedProviders.Decorators;

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
