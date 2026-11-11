using FixtureBuilder.Creation.UninitializedProviders.MemberInitialization;
using FixtureBuilder.Creation.UninitializedProviders.MemberResolution;

namespace FixtureBuilder.Creation.UninitializedProviders
{
    internal static class UninitializedProviderFactory
    {
        public static IRootUninitializedProvider CreateDefaultUninitializedProvider()
        {
            var creator = new RootUninitializedProvider(
                new MemberInitializer(
                    new DataMemberSkipFilter(),
                    new CompositeUninitializedProvider(
                        new DefaultBclTypeProvider())));

            return creator;
        }
    }
}
