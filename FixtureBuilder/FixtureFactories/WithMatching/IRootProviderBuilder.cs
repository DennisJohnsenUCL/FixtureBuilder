using FixtureBuilder.Core;

namespace FixtureBuilder.FixtureFactories.WithMatching
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
