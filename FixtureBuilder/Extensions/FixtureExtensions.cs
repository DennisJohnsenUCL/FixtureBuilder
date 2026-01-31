using System.Linq.Expressions;

namespace FixtureBuilder.Extensions
{
    public static class FixtureExtensions
    {
        public static TEntity With<TEntity, TProp>(this TEntity fixture, Expression<Func<TEntity, TProp>> expr, TProp value) where TEntity : class
        {
            return Fixture.New(fixture).With(expr, value).Build();
        }

        public static TEntity WithField<TEntity, TProp>(this TEntity fixture, Expression<Func<TEntity, TProp>> expr, TProp value) where TEntity : class
        {
            return Fixture.New(fixture).WithBackingField(expr, value).Build();
        }

        public static TEntity WithField<TEntity>(this TEntity fixture, string fieldName, object value) where TEntity : class
        {
            return Fixture.New(fixture).WithField(fieldName, value).Build();
        }

        public static TEntity WithField<TEntity, T>(this TEntity fixture, string fieldName, IEnumerable<T> values) where TEntity : class
        {
            return Fixture.New(fixture).WithField(fieldName, values).Build();
        }

        public static TEntity WithField<TEntity, TProp>(this TEntity fixture, string fieldName, Expression<Func<TEntity, TProp>> expr, TProp value) where TEntity : class
        {
            return Fixture.New(fixture).WithBackingField(fieldName, expr, value).Build();
        }

        public static TEntity WithSetter<TEntity, TProp>(this TEntity fixture, Expression<Func<TEntity, TProp>> expr, TProp value) where TEntity : class
        {
            return Fixture.New(fixture).WithSetter(expr, value).Build();
        }
    }
}
