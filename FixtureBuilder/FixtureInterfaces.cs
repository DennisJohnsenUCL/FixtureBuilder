using System.Linq.Expressions;

namespace FixtureBuilder
{
    public interface IFixtureConstructor<TEntity> : IFixtureConfigurator<TEntity> where TEntity : class
    {
        IFixtureConfigurator<TEntity> BypassConstructor();
        IFixtureConfigurator<TEntity> UseConstructor(params object[] args);
    }

    public interface IFixtureConfigurator<TEntity> where TEntity : class
    {
        IFixtureConfigurator<TTarget> CastTo<TTarget>() where TTarget : class;
        IFixtureConfigurator<TEntity> With<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value);
        IFixtureConfigurator<TEntity> WithField<T>(string fieldName, T value);
        IFixtureConfigurator<TEntity> WithField<T>(string fieldName, Expression<Func<TEntity, object?>> expr, T value);
        IFixtureConfigurator<TEntity> WithBackingField<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value);
        IFixtureConfigurator<TEntity> WithBackingField<TProp>(string fieldName, Expression<Func<TEntity, TProp>> expr, TProp value);
        IFixtureConfigurator<TEntity> WithSetter<TProp>(Expression<Func<TEntity, TProp>> expr, TProp value);
        bool HasField(string fieldName);
        bool HasField(string fieldName, Expression<Func<TEntity, object?>> expr);
        TEntity Build();
    }
}
