using Bogus;
using FixtureBuilder.FixtureFactories.WithMatching;

namespace FixtureBuilder.Bogus.BogusFixtureFactories
{
    /// <summary>
    /// Provides fluent methods for registering values and factory functions to be used during value resolution, scoped by member kind and name,
    /// with additional support for <see cref="Faker"/>-based data generation.
    /// </summary>
    /// <typeparam name="TReturn">The return type for fluent chaining.</typeparam>
    internal interface IBogusProviderBuilder<TReturn>
    {
        #region IBogusProviderBuilder

        /// <summary>
        /// Registers a <see cref="Faker"/>-based factory function to be invoked for all members and parameters of type <typeparamref name="T"/>.
        /// </summary>
        TReturn With<T>(Func<Faker, T> func);

        /// <summary>
        /// Registers a <see cref="Faker"/>-based factory function to be invoked for all members and parameters of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        TReturn With<T>(Func<Faker, T> func, string name);

        /// <summary>
        /// Registers a <see cref="Faker"/>-based factory function to be invoked for constructor parameters of type <typeparamref name="T"/>.
        /// </summary>
        TReturn WithParameter<T>(Func<Faker, T> func);

        /// <summary>
        /// Registers a <see cref="Faker"/>-based factory function to be invoked for constructor parameters of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        TReturn WithParameter<T>(Func<Faker, T> func, string name);

        /// <summary>
        /// Registers a <see cref="Faker"/>-based factory function to be invoked for properties and fields of type <typeparamref name="T"/>.
        /// </summary>
        TReturn WithPropertyOrField<T>(Func<Faker, T> func);

        /// <summary>
        /// Registers a <see cref="Faker"/>-based factory function to be invoked for properties and fields of type <typeparamref name="T"/> with a matching name.
        /// </summary>
        TReturn WithPropertyOrField<T>(Func<Faker, T> func, string name);

        #endregion

        #region IProviderBuilder

        /// <inheritdoc cref="IProviderBuilder{TReturn}.With{T}(T)"/>
        TReturn With<T>(T value);

        /// <inheritdoc cref="IProviderBuilder{TReturn}.With{T}(Func{T})"/>
        TReturn With<T>(Func<T> func);

        /// <inheritdoc cref="IProviderBuilder{TReturn}.With{T}(T, string)"/>
        TReturn With<T>(T value, string name);

        /// <inheritdoc cref="IProviderBuilder{TReturn}.With{T}(Func{T}, string)"/>
        TReturn With<T>(Func<T> func, string name);

        /// <inheritdoc cref="IProviderBuilder{TReturn}.WithParameter{T}(T)"/>
        TReturn WithParameter<T>(T value);

        /// <inheritdoc cref="IProviderBuilder{TReturn}.WithParameter{T}(Func{T})"/>
        TReturn WithParameter<T>(Func<T> func);

        /// <inheritdoc cref="IProviderBuilder{TReturn}.WithParameter{T}(T, string)"/>
        TReturn WithParameter<T>(T value, string name);

        /// <inheritdoc cref="IProviderBuilder{TReturn}.WithParameter{T}(Func{T}, string)"/>
        TReturn WithParameter<T>(Func<T> func, string name);

        /// <inheritdoc cref="IProviderBuilder{TReturn}.WithPropertyOrField{T}(T)"/>
        TReturn WithPropertyOrField<T>(T value);

        /// <inheritdoc cref="IProviderBuilder{TReturn}.WithPropertyOrField{T}(Func{T})"/>
        TReturn WithPropertyOrField<T>(Func<T> func);

        /// <inheritdoc cref="IProviderBuilder{TReturn}.WithPropertyOrField{T}(T, string)"/>
        TReturn WithPropertyOrField<T>(T value, string name);

        /// <inheritdoc cref="IProviderBuilder{TReturn}.WithPropertyOrField{T}(Func{T}, string)"/>
        TReturn WithPropertyOrField<T>(Func<T> func, string name);

        #endregion
    }
}
