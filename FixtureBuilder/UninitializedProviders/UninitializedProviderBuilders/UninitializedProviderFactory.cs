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
                        new CompositeUninitializedProvider([
                            new StringProvider(),
                            new EnumProvider(),
                            new DateTimeProvider(),
                            new TimeSpanProvider(),
                            new PrimitiveNumberProvider(),
                            new ArrayProvider(),
                            new ConstructibleEnumerableProvider(),
                            new ImmutableFrozenEnumerableProvider(),
                            new DefaultBclTypeProvider()]))));

            return creator;
        }
    }
}
