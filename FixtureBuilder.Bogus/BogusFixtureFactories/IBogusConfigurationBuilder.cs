using Bogus;
using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories;

namespace FixtureBuilder.Bogus.BogusFixtureFactories
{
    /// <summary>
    /// Provides registration methods for options, type links, value providers, and value converters within a configuration scope,
    /// with additional support for Bogus-integrated providers.
    /// </summary>
    /// <typeparam name="TReturn">The return type for fluent chaining from inherited <see cref="IBogusProviderBuilder{TReturn}"/> methods.</typeparam>
    internal interface IBogusConfigurationBuilder<TReturn>
    {
        #region IBogusConfigurationBuilder

        /// <summary>
        /// Registers a Bogus-integrated custom provider for value resolution.
        /// </summary>
        /// <remarks>
        /// The provider receives a <see cref="Faker"/> instance for data generation.
        /// The provider must return new <see cref="NoResult"/>() for requests it does not handle. A <see langword="null"/> return is treated as an explicit null assignment.
        /// </remarks>
        TReturn AddBogusProvider(IBogusCustomProvider provider);

        #endregion

        #region IConfigurationBuilder

        /// <inheritdoc cref="IConfigurationBuilder{TReturn}.Options"/>
        FixtureOptions Options { set; }

        /// <inheritdoc cref="IConfigurationBuilder{TReturn}.SetOptions"/>
        TReturn SetOptions(Action<FixtureOptions> optionsAction);

        /// <inheritdoc cref="IConfigurationBuilder{TReturn}.AddProvider"/>
        TReturn AddProvider(ICustomProvider provider);

        /// <inheritdoc cref="IConfigurationBuilder{TReturn}.AddConverter"/>
        TReturn AddConverter(ICustomConverter converter);

        /// <inheritdoc cref="IConfigurationBuilder{TReturn}.AddTypeLink{TIn, TOut}"/>
        TReturn AddTypeLink<TIn, TOut>();

        /// <inheritdoc cref="IConfigurationBuilder{TReturn}.AddTypeLink(Type, Type)"/>
        TReturn AddTypeLink(Type typeIn, Type typeOut);

        /// <inheritdoc cref="IConfigurationBuilder{TReturn}.AddTypeLink(ICustomTypeLink)"/>
        TReturn AddTypeLink(ICustomTypeLink typeLink);

        #endregion
    }
}
