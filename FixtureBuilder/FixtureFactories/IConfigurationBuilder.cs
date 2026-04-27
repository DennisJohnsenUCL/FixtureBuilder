using FixtureBuilder.Core;
using FixtureBuilder.FixtureFactories.WithMatching;

namespace FixtureBuilder.FixtureFactories
{
    public interface IConfigurationBuilder<TReturn> : IProviderBuilder<TReturn>
    {
        /// <summary>
        /// Sets the options for this configuration scope.
        /// </summary>
        FixtureOptions Options { set; }

        /// <summary>
        /// Configures the options for this configuration scope via a mutating action.
        /// </summary>
        void SetOptions(Action<FixtureOptions> optionsAction);

        /// <summary>
        /// Registers a custom provider for value resolution.
        /// </summary>
        /// <remarks>
        /// The provider must return <c>new NoResult()</c> for requests it does not handle. A <c>null</c> return is treated as an explicit null assignment.
        /// </remarks>
        void AddProvider(ICustomProvider provider);

        /// <summary>
        /// Registers a custom converter for backing field assignment when the field type differs from the property type.
        /// </summary>
        /// <remarks>
        /// The converter must return <c>new NoResult()</c> for conversions it does not handle. A <c>null</c> return is treated as an explicit null assignment.
        /// </remarks>
        void AddConverter(ICustomConverter converter);

        /// <summary>
        /// Registers a type link that substitutes <typeparamref name="TIn"/> with <typeparamref name="TOut"/> during value resolution.
        /// </summary>
        void AddTypeLink<TIn, TOut>();

        /// <summary>
        /// Registers a type link that substitutes <paramref name="typeIn"/> with <paramref name="typeOut"/> during value resolution.
        /// </summary>
        void AddTypeLink(Type typeIn, Type typeOut);

        /// <summary>
        /// Registers a custom type link for type substitution during value resolution.
        /// </summary>
        void AddTypeLink(ICustomTypeLink typeLink);
    }
}
