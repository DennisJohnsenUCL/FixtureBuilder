using System.Linq.Expressions;
using Bogus;
using MemberLens.Attributes;

namespace FixtureBuilder.Bogus
{
    /// <summary>
    /// Provides configuration methods for a Bogus-integrated fixture of type <typeparamref name="T"/>.
    /// Each method accepts either a flat value (passthrough to FixtureBuilder) or a <see cref="Faker"/> lambda for data generation.
    /// Configuration is deferred until <see cref="Build"/> or <see cref="Build(int)"/> is called.
    /// </summary>
    /// <typeparam name="T">The type of the object to configure. Must be a reference type.</typeparam>
    public interface IBogusFixtureConfigurator<T> where T : class
    {
        #region IBogusFixtureConfigurator

        /// <summary>
        /// Instantiates the specified property or field using a caller-defined instantiation strategy with access to a <see cref="Faker"/>-integrated constructor.
        /// </summary>
        /// <param name="expr">An expression identifying the property or field to instantiate. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="func">A function that receives an <see cref="IBogusConstructor{TProp}"/> and returns an instance of <typeparamref name="TProp"/>.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IBogusFixtureConfigurator<T> Instantiate<TProp>(Expression<Func<T, TProp>> expr, Func<IBogusConstructor<TProp>, TProp> func);

        /// <summary>
        /// Configures the fixture by setting the specified field to a value resolved via a <see cref="Faker"/> lambda.
        /// </summary>
        /// <remarks>
        /// This method supports the MemberLens VS extension which will provide field names automatically if installed.
        /// </remarks>
        /// <param name="fieldName">The name of the field to set. This must match the name of an existing field on <typeparamref name="T"/>.</param>
        /// <param name="value">A function that receives a <see cref="Faker"/> and returns the value to assign to the specified field.</param>
        /// <returns>The current configurator for further configuration.</returns>
        IBogusFixtureConfigurator<T> WithField<TValue>([MemberAccessor(AccessorType.Field, GenericSource.Class, 0)] string fieldName, Func<Faker, TValue> value);

        /// <summary>
        /// Configures the fixture by setting the specified field to a value resolved via a <see cref="Faker"/> lambda on a nested property or field.
        /// </summary>
        /// <remarks>
        /// This method supports the MemberLens VS extension which will provide field names automatically if installed.
        /// </remarks>
        /// <param name="expr">An expression identifying the nested property or field on which to set the field. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="fieldName">The name of the field to set. This must match the name of an existing field on the resolved member.</param>
        /// <param name="value">A function that receives a <see cref="Faker"/> and returns the value to assign to the specified field.</param>
        /// <returns>The current configurator for further configuration.</returns>
        IBogusFixtureConfigurator<T> WithField<TProp, TValue>(Expression<Func<T, TProp>> expr, [MemberAccessor(AccessorType.Field, GenericSource.Method, 0)] string fieldName, Func<Faker, TValue> value);

        /// <summary>
        /// Configures the fixture by setting the backing field of the specified property to a value resolved via a <see cref="Faker"/> lambda.
        /// </summary>
        /// <param name="expr">An expression identifying the property whose backing field should be set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">A function that receives a <see cref="Faker"/> and returns the value to assign to the backing field.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// The backing field is automatically discovered using common naming conventions.
        /// Supports both regular properties and interface-implemented properties with explicit backing fields.
        /// </remarks>
        IBogusFixtureConfigurator<T> WithBackingField<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, TProp> value);

        /// <summary>
        /// Configures the fixture by setting a specifically named backing field of the specified property to a value resolved via a <see cref="Faker"/> lambda.
        /// </summary>
        /// <param name="expr">An expression identifying the property whose backing field should be set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">A function that receives a <see cref="Faker"/> and returns the value to assign to the backing field.</param>
        /// <param name="fieldName">The explicit name of the backing field to set.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// Use this overload when the backing field name cannot be automatically discovered.
        /// </remarks>
        IBogusFixtureConfigurator<T> WithBackingField<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, TProp> value, string fieldName);

        /// <summary>
        /// Configures the fixture by setting the backing field of the specified property to a value resolved via a <see cref="Faker"/> lambda, without enforcing the property's type.
        /// </summary>
        /// <param name="expr">An expression identifying the property whose backing field should be set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">A function that receives a <see cref="Faker"/> and returns the value to assign to the backing field.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// Use this overload when the backing field has a different type from the property.
        /// The backing field is automatically discovered using common naming conventions.
        /// Supports both regular properties and interface-implemented properties with explicit backing fields.
        /// </remarks>
        IBogusFixtureConfigurator<T> WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, object?> value);

        /// <summary>
        /// Configures the fixture by setting a specifically named backing field of the specified property to a value resolved via a <see cref="Faker"/> lambda, without enforcing the property's type.
        /// </summary>
        /// <param name="expr">An expression identifying the property whose backing field should be set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">A function that receives a <see cref="Faker"/> and returns the value to assign to the backing field.</param>
        /// <param name="fieldName">The explicit name of the backing field to set.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// Use this overload when the backing field has a different type from the property and its name cannot be automatically discovered.
        /// </remarks>
        IBogusFixtureConfigurator<T> WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, object?> value, string fieldName);

        /// <summary>
        /// Configures the fixture by setting the specified property through its setter to a value resolved via a <see cref="Faker"/> lambda.
        /// </summary>
        /// <param name="expr">An expression identifying the property to set. The property must have a setter. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">A function that receives a <see cref="Faker"/> and returns the value to assign to the property.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IBogusFixtureConfigurator<T> WithSetter<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, TProp> value);

        /// <summary>
        /// Configures the fixture by setting the specified property or field to a value resolved via a <see cref="Faker"/> lambda.
        /// </summary>
        /// <param name="expr">An expression identifying the property or field to set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">A function that receives a <see cref="Faker"/> and returns the value to assign.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// Automatically determines whether to use the property setter (if writable) or directly set the backing field (if read-only).
        /// </remarks>
        IBogusFixtureConfigurator<T> With<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, TProp> value);

        /// <summary>
        /// Invokes a method on the fixture determined by a <see cref="Faker"/> lambda that produces the method call expression.
        /// </summary>
        /// <param name="expr">A function that receives a <see cref="Faker"/> and returns a lambda expression representing a method call on the fixture.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the expression is not a valid method call on a property or field access chain.</exception>
        IBogusFixtureConfigurator<T> Invoke(Func<Faker, Expression<Action<T>>> expr);

        /// <summary>
        /// Invokes a method by name on the fixture using reflection, with arguments resolved via a <see cref="Faker"/> lambda.
        /// </summary>
        /// <remarks>
        /// This method supports the MemberLens VS extension which will provide method names automatically if installed.
        /// </remarks>
        /// <param name="methodName">The name of the method to invoke on the fixture.</param>
        /// <param name="arguments">A function that receives a <see cref="Faker"/> and returns the arguments to pass to the method.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the method is not found on <typeparamref name="T"/>.</exception>
        IBogusFixtureConfigurator<T> InvokePrivate([MemberAccessor(AccessorType.Method, GenericSource.Class, 0)] string methodName, Func<Faker, object[]> arguments);

        /// <summary>
        /// Invokes a method by name on a nested property or field of the fixture using reflection, with arguments resolved via a <see cref="Faker"/> lambda.
        /// </summary>
        /// <remarks>
        /// This method supports the MemberLens VS extension which will provide method names automatically if installed.
        /// </remarks>
        /// <param name="expr">An expression identifying the nested property or field. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="methodName">The name of the method to invoke on the resolved member.</param>
        /// <param name="arguments">A function that receives a <see cref="Faker"/> and returns the arguments to pass to the method.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the expression is invalid or the method is not found on <typeparamref name="TProp"/>.</exception>
        IBogusFixtureConfigurator<T> InvokePrivate<TProp>(Expression<Func<T, TProp>> expr, [MemberAccessor(AccessorType.Method, GenericSource.Method, 0)] string methodName, Func<Faker, object[]> arguments);

        /// <summary>
        /// Builds and returns the specified number of configured fixture instances, each with independently generated values.
        /// </summary>
        /// <param name="count">The number of instances to build. Must be greater than zero.</param>
        /// <returns>A collection of configured fixture instances of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        IEnumerable<T> Build(int count);

        #endregion

        #region IFixtureConfigurator

        /// <summary>
        /// Instantiates the specified property or field using the default instantiation method and assigns it to the fixture.
        /// </summary>
        /// <param name="expr">An expression identifying the property or field to instantiate. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// The default instantiation method can be set via the <see cref="FixtureOptions.DefaultInstantiateInstantiationMethod"/> option.
        /// If the property has a setter, the value is assigned through it; otherwise, the backing field is set directly.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        IBogusFixtureConfigurator<T> Instantiate<TProp>(Expression<Func<T, TProp>> expr);

        /// <summary>
        /// Configures the fixture by setting the specified field to the given value.
        /// </summary>
        /// <remarks>
        /// This method supports the MemberLens VS extension which will provide field names automatically if installed.
        /// </remarks>
        /// <param name="fieldName">The name of the field to set. This must match the name of an existing field on <typeparamref name="T"/>.</param>
        /// <param name="value">The value to assign to the specified field.</param>
        /// <returns>The current configurator for further configuration.</returns>
        IBogusFixtureConfigurator<T> WithField<TValue>([MemberAccessor(AccessorType.Field, GenericSource.Class, 0)] string fieldName, TValue value);

        /// <summary>
        /// Configures the fixture by setting the specified field to the given value on a nested property or field.
        /// </summary>
        /// <remarks>
        /// This method supports the MemberLens VS extension which will provide field names automatically if installed.
        /// </remarks>
        /// <param name="expr">An expression identifying the nested property or field on which to set the field. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="fieldName">The name of the field to set. This must match the name of an existing field on the resolved member.</param>
        /// <param name="value">The value to assign to the specified field.</param>
        /// <returns>The current configurator for further configuration.</returns>
        IBogusFixtureConfigurator<T> WithField<TProp, TValue>(Expression<Func<T, TProp>> expr, [MemberAccessor(AccessorType.Field, GenericSource.Method, 0)] string fieldName, TValue value);

        /// <summary>
        /// Configures the fixture by setting the backing field of the specified property.
        /// </summary>
        /// <param name="expr">An expression identifying the property whose backing field should be set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">The value to assign to the backing field.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// The backing field is automatically discovered using common naming conventions.
        /// Supports both regular properties and interface-implemented properties with explicit backing fields.
        /// </remarks>
        IBogusFixtureConfigurator<T> WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value);

        /// <summary>
        /// Configures the fixture by setting a specifically named backing field of the specified property.
        /// </summary>
        /// <param name="expr">An expression identifying the property whose backing field should be set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">The value to assign to the backing field.</param>
        /// <param name="fieldName">The explicit name of the backing field to set.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// Use this overload when the backing field name cannot be automatically discovered.
        /// </remarks>
        IBogusFixtureConfigurator<T> WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value, string fieldName);

        /// <summary>
        /// Configures the fixture by setting the backing field of the specified property without enforcing the property's type.
        /// </summary>
        /// <param name="expr">An expression identifying the property whose backing field should be set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">The value to assign to the backing field.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// Use this overload when the backing field has a different type from the property.
        /// The backing field is automatically discovered using common naming conventions.
        /// Supports both regular properties and interface-implemented properties with explicit backing fields.
        /// </remarks>
        IBogusFixtureConfigurator<T> WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value);

        /// <summary>
        /// Configures the fixture by setting a specifically named backing field of the specified property without enforcing the property's type.
        /// </summary>
        /// <param name="expr">An expression identifying the property whose backing field should be set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">The value to assign to the backing field.</param>
        /// <param name="fieldName">The explicit name of the backing field to set.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// Use this overload when the backing field has a different type from the property and its name cannot be automatically discovered.
        /// </remarks>
        IBogusFixtureConfigurator<T> WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value, string fieldName);

        /// <summary>
        /// Configures the fixture by setting the specified property through its setter.
        /// </summary>
        /// <param name="expr">An expression identifying the property to set. The property must have a setter. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">The value to assign to the property.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IBogusFixtureConfigurator<T> WithSetter<TProp>(Expression<Func<T, TProp>> expr, TProp value);

        /// <summary>
        /// Configures the fixture by setting the specified property or field.
        /// </summary>
        /// <param name="expr">An expression identifying the property or field to set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">The value to assign.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// Automatically determines whether to use the property setter (if writable) or directly set the backing field (if read-only).
        /// </remarks>
        IBogusFixtureConfigurator<T> With<TProp>(Expression<Func<T, TProp>> expr, TProp value);

        /// <summary>
        /// Invokes a method on the fixture or a nested property or field via an expression.
        /// </summary>
        /// <param name="expr">A lambda expression representing a method call on the fixture, e.g. <c>x => x.Child.DoSomething()</c>. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the expression is not a valid method call on a property or field access chain.</exception>
        IBogusFixtureConfigurator<T> Invoke(Expression<Action<T>> expr);

        /// <summary>
        /// Invokes a method by name on the fixture using reflection, allowing access to private and protected methods.
        /// </summary>
        /// <remarks>
        /// This method supports the MemberLens VS extension which will provide method names automatically if installed.
        /// </remarks>
        /// <param name="methodName">The name of the method to invoke on the fixture.</param>
        /// <param name="arguments">Arguments to pass to the method.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the method is not found on <typeparamref name="T"/>.</exception>
        IBogusFixtureConfigurator<T> InvokePrivate([MemberAccessor(AccessorType.Method, GenericSource.Class, 0)] string methodName, params object[] arguments);

        /// <summary>
        /// Invokes a method by name on a nested property or field of the fixture using reflection, allowing access to private and protected methods.
        /// </summary>
        /// <remarks>
        /// This method supports the MemberLens VS extension which will provide method names automatically if installed.
        /// </remarks>
        /// <param name="expr">An expression identifying the nested property or field. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="methodName">The name of the method to invoke on the resolved member.</param>
        /// <param name="arguments">Arguments to pass to the method.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the expression is invalid or the method is not found on <typeparamref name="TProp"/>.</exception>
        IBogusFixtureConfigurator<T> InvokePrivate<TProp>(Expression<Func<T, TProp>> expr, [MemberAccessor(AccessorType.Method, GenericSource.Method, 0)] string methodName, params object[] arguments);

        /// <summary>
        /// Builds and returns the configured fixture instance.
        /// </summary>
        /// <remarks>
        /// Each call creates a new instance with freshly generated values.
        /// </remarks>
        /// <returns>The configured fixture instance of type <typeparamref name="T"/>.</returns>
        T Build();

        #endregion
    }
}
