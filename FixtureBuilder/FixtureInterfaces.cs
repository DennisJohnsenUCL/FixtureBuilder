using System.Linq.Expressions;
using FixtureBuilder.UninitializedProviders;
using MemberLens.Attributes;

namespace FixtureBuilder
{
    public interface IFixtureConstructor<TEntity> : IFixtureConfigurator<TEntity> where TEntity : class
    {
        IFixtureConfigurator<TEntity> CreateUninitialized();
        IFixtureConfigurator<TEntity> CreateUninitialized(InitializeMembers initializeMembers);
        IFixtureConfigurator<TEntity> UseConstructor(params object[] args);
    }

    public interface IFixtureConfigurator<TEntity> where TEntity : class
    {
        IFixtureConfigurator<TTarget> CastTo<TTarget>() where TTarget : class;
        IFixtureConfigurator<TEntity> With<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value);
        IFixtureConfigurator<TEntity> WithField<T>([MemberAccessor(AccessorType.Field, GenericSource.Class, 0)] string fieldName, T value);
        IFixtureConfigurator<TEntity> WithField<TProp, T>(Expression<Func<TEntity, TProp>> expr, [MemberAccessor(AccessorType.Field, GenericSource.Method, 0)] string fieldName, T value);
        IFixtureConfigurator<TEntity> WithBackingField<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value);
        IFixtureConfigurator<TEntity> WithBackingField<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value, string fieldName);
        IFixtureConfigurator<TEntity> WithBackingFieldUntyped<TProp>(Expression<Func<TEntity, TProp>> expr, object? value);
        IFixtureConfigurator<TEntity> WithBackingFieldUntyped<TProp>(Expression<Func<TEntity, TProp>> expr, object? value, string fieldName);
        IFixtureConfigurator<TEntity> WithSetter<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value);
        TEntity Build();
    }
}
