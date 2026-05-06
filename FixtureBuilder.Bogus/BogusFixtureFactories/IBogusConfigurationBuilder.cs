using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories;

namespace FixtureBuilder.Bogus.BogusFixtureFactories
{
    internal interface IBogusConfigurationBuilder<TReturn>
    {
        #region IBogusConfigurationBuilder

        TReturn AddBogusProvider(IBogusCustomProvider provider);

        #endregion

        #region IConfigurationBuilder

        FixtureOptions Options { set; }

        TReturn SetOptions(Action<FixtureOptions> optionsAction);

        TReturn AddProvider(ICustomProvider provider);

        TReturn AddConverter(ICustomConverter converter);

        TReturn AddTypeLink<TIn, TOut>();

        TReturn AddTypeLink(Type typeIn, Type typeOut);

        TReturn AddTypeLink(ICustomTypeLink typeLink);

        #endregion
    }
}
