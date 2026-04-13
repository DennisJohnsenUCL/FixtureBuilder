using FixtureBuilder.Assignment.ValueProviders.Providers;

namespace FixtureBuilder.Creation.UninitializedProviders
{
    internal class UninitializedProviderFactory
    {
        public static IUninitializedProvider CreateDefaultUninitializedProvider()
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
