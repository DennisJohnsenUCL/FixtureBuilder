using System.Linq.Expressions;
using Bogus;
using MemberLens.Attributes;

namespace FixtureBuilder.Bogus
{
    public interface IBogusFixtureConfigurator<T> where T : class
    {
        #region IBogusFixtureConfigurator

        IBogusFixtureConfigurator<T> Instantiate<TProp>(Expression<Func<T, TProp>> expr, Func<IBogusConstructor<TProp>, TProp> func);

        IBogusFixtureConfigurator<T> WithField<TValue>([MemberAccessor(AccessorType.Field, GenericSource.Class, 0)] string fieldName, Func<Faker, TValue> value);

        IBogusFixtureConfigurator<T> WithField<TProp, TValue>(Expression<Func<T, TProp>> expr, [MemberAccessor(AccessorType.Field, GenericSource.Method, 0)] string fieldName, Func<Faker, TValue> value);

        IBogusFixtureConfigurator<T> WithBackingField<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, TProp> value);

        IBogusFixtureConfigurator<T> WithBackingField<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, TProp> value, string fieldName);

        IBogusFixtureConfigurator<T> WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, object?> value);

        IBogusFixtureConfigurator<T> WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, object?> value, string fieldName);

        IBogusFixtureConfigurator<T> WithSetter<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, TProp> value);

        IBogusFixtureConfigurator<T> With<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, TProp> value);

        IBogusFixtureConfigurator<T> Invoke(Func<Faker, Expression<Action<T>>> expr);

        IBogusFixtureConfigurator<T> InvokePrivate([MemberAccessor(AccessorType.Method, GenericSource.Class, 0)] string methodName, Func<Faker, object[]> arguments);

        IBogusFixtureConfigurator<T> InvokePrivate<TProp>(Expression<Func<T, TProp>> expr, [MemberAccessor(AccessorType.Method, GenericSource.Method, 0)] string methodName, Func<Faker, object[]> arguments);

        IEnumerable<T> Build(int count);

        #endregion

        #region IFixtureConfigurator

        IBogusFixtureConfigurator<T> Instantiate<TProp>(Expression<Func<T, TProp>> expr);

        IBogusFixtureConfigurator<T> WithField<TValue>([MemberAccessor(AccessorType.Field, GenericSource.Class, 0)] string fieldName, TValue value);

        IBogusFixtureConfigurator<T> WithField<TProp, TValue>(Expression<Func<T, TProp>> expr, [MemberAccessor(AccessorType.Field, GenericSource.Method, 0)] string fieldName, TValue value);

        IBogusFixtureConfigurator<T> WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value);

        IBogusFixtureConfigurator<T> WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value, string fieldName);

        IBogusFixtureConfigurator<T> WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value);

        IBogusFixtureConfigurator<T> WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value, string fieldName);

        IBogusFixtureConfigurator<T> WithSetter<TProp>(Expression<Func<T, TProp>> expr, TProp value);

        IBogusFixtureConfigurator<T> With<TProp>(Expression<Func<T, TProp>> expr, TProp value);

        IBogusFixtureConfigurator<T> Invoke(Expression<Action<T>> expr);

        IBogusFixtureConfigurator<T> InvokePrivate([MemberAccessor(AccessorType.Method, GenericSource.Class, 0)] string methodName, params object[] arguments);

        IBogusFixtureConfigurator<T> InvokePrivate<TProp>(Expression<Func<T, TProp>> expr, [MemberAccessor(AccessorType.Method, GenericSource.Method, 0)] string methodName, params object[] arguments);

        T Build();

        #endregion
    }
}
