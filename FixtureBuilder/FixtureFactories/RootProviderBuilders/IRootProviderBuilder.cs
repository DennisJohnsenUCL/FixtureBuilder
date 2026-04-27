using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories.WithMatching;

namespace FixtureBuilder.FixtureFactories.RootProviderBuilders
{
    public interface IRootProviderBuilder<TRoot> : IProviderBuilder<IRootProviderBuilder<TRoot>>
    {
        FixtureOptions Options { set; }
        void SetOptions(Action<FixtureOptions> optionsAction);
        void AddProvider(ICustomProvider provider);
        void AddConverter(ICustomConverter converter);
        void AddTypeLink<TIn, TOut>();
        void AddTypeLink(Type typeIn, Type typeOut);
        void AddTypeLink(ICustomTypeLink typeLink);
    }
}
