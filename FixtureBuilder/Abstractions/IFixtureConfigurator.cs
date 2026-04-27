using System.Linq.Expressions;
using FixtureBuilder.Core;
using MemberLens.Attributes;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public interface IFixtureConfigurator<T> where T : class
    {
        /// <summary>
        /// Casts the current fixture to the specified target type.
        /// </summary>
        /// <typeparam name="TTarget">The type to which the fixture should be cast.</typeparam>
        /// <returns>The current configurator for further configuration.</returns>
        /// <exception cref="InvalidCastException"/>
        IFixtureConfigurator<TTarget> CastTo<TTarget>() where TTarget : class;

        /// <summary>
        /// Instantiates the specified property or field using the default instantiation method and assigns it to the fixture.
        /// </summary>
        /// <param name="expr">An expression identifying the property or field to instantiate. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <remarks>
        /// The default instantiation method can be set via the <see cref="FixtureOptions.DefaultInstantiateInstantiationMethod"/> option.
        /// If the property has a setter, the value is assigned through it; otherwise, the backing field is set directly.
        /// </remarks>
        /// <returns>The current configurator for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<T> Instantiate<TProp>(Expression<Func<T, TProp>> expr);

        /// <summary>
        /// Instantiates the specified property or field using a caller-defined instantiation strategy and assigns it to the fixture.
        /// </summary>
        /// <param name="expr">An expression identifying the property or field to instantiate. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="func">A function that receives an <see cref="IConstructor{TProp}"/> and returns an instance of <typeparamref name="TProp"/>.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// If the property has a setter, the value is assigned through it; otherwise, the backing field is set directly.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<T> Instantiate<TProp>(Expression<Func<T, TProp>> expr, Func<IConstructor<TProp>, TProp> func);

        /// <summary>
        /// Configures the fixture by setting the specified field to the given value.
        /// </summary>
        /// <remarks>
        /// This method supports the MemberLens VS extension which will provide field names automatically if installed.
        /// </remarks>
        /// <param name="fieldName">The name of the field to set. This must match the name of an existing field on <typeparamref name="T"/>.</param>
        /// <param name="value">The value to assign to the specified field.</param>
        /// <returns>The current configurator for further configuration.</returns>
        IFixtureConfigurator<T> WithField<TValue>([MemberAccessor(AccessorType.Field, GenericSource.Class, 0)] string fieldName, TValue value);

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
        IFixtureConfigurator<T> WithField<TProp, TValue>(Expression<Func<T, TProp>> expr, [MemberAccessor(AccessorType.Field, GenericSource.Method, 0)] string fieldName, TValue value);

        /// <summary>
        /// Configures the fixture by setting the backing field of the specified property.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="expr">An expression identifying the property whose backing field should be set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">The value to assign to the backing field.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// The backing field is automatically discovered using common naming conventions.
        /// Supports both regular properties and interface-implemented properties with explicit backing fields.
        /// </remarks>
        IFixtureConfigurator<T> WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value);

        /// <summary>
        /// Configures the fixture by setting a specifically named backing field of the specified property.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="expr">An expression identifying the property whose backing field should be set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">The value to assign to the backing field.</param>
        /// <param name="fieldName">The explicit name of the backing field to set.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// Use this overload when the backing field name cannot be automatically discovered.
        /// </remarks>
        IFixtureConfigurator<T> WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value, string fieldName);

        /// <summary>
        /// Configures the fixture by setting the backing field of the specified property without enforcing the property's type.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="expr">An expression identifying the property whose backing field should be set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">The value to assign to the backing field.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// Use this overload when the backing field has a different type from the property.
        /// The backing field is automatically discovered using common naming conventions.
        /// Supports both regular properties and interface-implemented properties with explicit backing fields.
        /// </remarks>
        IFixtureConfigurator<T> WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value);

        /// <summary>
        /// Configures the fixture by setting a specifically named backing field of the specified property without enforcing the property's type.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="expr">An expression identifying the property whose backing field should be set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">The value to assign to the backing field.</param>
        /// <param name="fieldName">The explicit name of the backing field to set.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// Use this overload when the backing field has a different type from the property and its name cannot be automatically discovered.
        /// </remarks>
        IFixtureConfigurator<T> WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value, string fieldName);

        /// <summary>
        /// Configures the fixture by setting the specified property through its setter.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="expr">An expression identifying the property to set. The property must have a setter. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">The value to assign to the property.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <exception cref="InvalidOperationException"/>
        IFixtureConfigurator<T> WithSetter<TProp>(Expression<Func<T, TProp>> expr, TProp value);

        /// <summary>
        /// Configures the fixture by setting the specified property or field.
        /// </summary>
        /// <typeparam name="TProp">The type of the property or field.</typeparam>
        /// <param name="expr">An expression identifying the property or field to set. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="value">The value to assign.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <remarks>
        /// Automatically determines whether to use the property setter (if writable) or directly set the backing field (if read-only).
        /// </remarks>
        IFixtureConfigurator<T> With<TProp>(Expression<Func<T, TProp>> expr, TProp value);

        /// <summary>
        /// Invokes a method on the fixture or a nested property or field via an expression.
        /// </summary>
        /// <param name="expr">A lambda expression representing a method call on the fixture, e.g. <c>x => x.Child.DoSomething()</c>. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the expression is not a valid method call on a property or field access chain.</exception>
        IFixtureConfigurator<T> Invoke(Expression<Action<T>> expr);

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
        IFixtureConfigurator<T> InvokePrivate([MemberAccessor(AccessorType.Method, GenericSource.Class, 0)] string methodName, params object[] arguments);

        /// <summary>
        /// Invokes a method by name on a nested property or field of the fixture using reflection, allowing access to private and protected methods.
        /// </summary>
        /// <remarks>
        /// This method supports the MemberLens VS extension which will provide method names automatically if installed.
        /// </remarks>
        /// <typeparam name="TProp">The type of the target property or field.</typeparam>
        /// <param name="expr">An expression identifying the nested property or field. Intermediate properties or fields in the chain are automatically initialized if allowed via <see cref="FixtureOptions.AllowInstantiateNestedMembers"/>.</param>
        /// <param name="methodName">The name of the method to invoke on the resolved member.</param>
        /// <param name="arguments">Arguments to pass to the method.</param>
        /// <returns>The current configurator for further configuration.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the expression is invalid or the method is not found on <typeparamref name="TProp"/>.</exception>
        IFixtureConfigurator<T> InvokePrivate<TProp>(Expression<Func<T, TProp>> expr, [MemberAccessor(AccessorType.Method, GenericSource.Method, 0)] string methodName, params object[] arguments);

        /// <summary>
        /// Builds and returns the configured fixture instance.
        /// </summary>
        /// <remarks>
        /// If the fixture has not been previously created, a new instance is instantiated using the <see cref="FixtureOptions.DefaultInstantiationMethod"/>. Subsequent calls return the same instance.
        /// </remarks>
        /// <returns>The configured fixture instance of type <typeparamref name="T"/>.</returns>
        T Build();
    }
}
