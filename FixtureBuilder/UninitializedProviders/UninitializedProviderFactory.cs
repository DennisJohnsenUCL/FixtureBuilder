using FixtureBuilder.UninitializedProviders.Decorators;
using FixtureBuilder.ValueProviders.Providers;

namespace FixtureBuilder.UninitializedProviders
{
    internal class UninitializedProviderFactory
    {
        public static IUninitializedProvider CreateDefaultUninitializedProvider()
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
