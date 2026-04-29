using System.Linq.Expressions;
using MemberLens.Attributes;

namespace FixtureBuilder.Bogus
{
    public interface IBogusFixtureConfigurator<T> where T : class
    {
        IBogusFixtureConfigurator<T> Instantiate<TProp>(Expression<Func<T, TProp>> expr);

        IBogusFixtureConfigurator<T> Instantiate<TProp>(Expression<Func<T, TProp>> expr, Func<IConstructor<TProp>, TProp> func);

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
    }
}
