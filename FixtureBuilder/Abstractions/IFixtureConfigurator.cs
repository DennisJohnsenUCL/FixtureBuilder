using System.Linq.Expressions;
using MemberLens.Attributes;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public interface IFixtureConfigurator<T> where T : class
    {
        IFixtureConfigurator<TTarget> CastTo<TTarget>() where TTarget : class;
        IFixtureConfigurator<T> Instantiate<TProp>(Expression<Func<T, TProp>> expr, Func<IConstructor<TProp>, TProp> func);
        IFixtureConfigurator<T> Instantiate<TProp>(Expression<Func<T, TProp>> expr);
        IFixtureConfigurator<T> With<TProp>(Expression<Func<T, TProp>> expr, TProp value);
        IFixtureConfigurator<T> WithField<TValue>([MemberAccessor(AccessorType.Field, GenericSource.Class, 0)] string fieldName, TValue value);
        IFixtureConfigurator<T> WithField<TProp, TValue>(Expression<Func<T, TProp>> expr, [MemberAccessor(AccessorType.Field, GenericSource.Method, 0)] string fieldName, TValue value);
        IFixtureConfigurator<T> WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value);
        IFixtureConfigurator<T> WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value, string fieldName);
        IFixtureConfigurator<T> WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value);
        IFixtureConfigurator<T> WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value, string fieldName);
        IFixtureConfigurator<T> WithSetter<TProp>(Expression<Func<T, TProp>> expr, TProp value);
        IFixtureConfigurator<T> Invoke(Expression<Action<T>> expr);
        IFixtureConfigurator<T> InvokePrivate([MemberAccessor(AccessorType.Method, GenericSource.Class, 0)] string methodName, params object[] arguments);
        IFixtureConfigurator<T> InvokePrivate<TProp>(Expression<Func<T, TProp>> expr, [MemberAccessor(AccessorType.Method, GenericSource.Method, 0)] string methodName, params object[] arguments);
        T Build();
    }
}
