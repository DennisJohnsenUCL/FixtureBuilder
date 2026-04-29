using System.Linq.Expressions;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Bogus
{
    internal class BogusFixture<T> : IBogusFixtureConstructor<T>, IBogusFixtureConfigurator<T> where T : class
    {
        private Func<IFixtureConstructor<T>, IFixtureConfigurator<T>>? _constructionCommand = null;
        private readonly List<Action<IFixtureConfigurator<T>>> _configurationCommands = [];

        public BogusFixture() { }

        T IBogusFixtureConfigurator<T>.Build()
        {
            var fixtureConstructor = Fixture.New<T>();

            IFixtureConfigurator<T> fixture = _constructionCommand != null
                ? _constructionCommand(fixtureConstructor)
                : fixtureConstructor;

            foreach (var command in _configurationCommands)
            {
                command(fixture);
            }
            return fixture.Build();
        }

        #region Passthrough construction methods

        IBogusFixtureConfigurator<T> IBogusFixtureConstructor<T>.CreateUninitialized()
        {
            _constructionCommand = f => f.CreateUninitialized();
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConstructor<T>.CreateUninitialized(InitializeMembers initializeMembers)
        {
            _constructionCommand = f => f.CreateUninitialized(initializeMembers);
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConstructor<T>.UseConstructor(params object[] args)
        {
            _constructionCommand = f => f.UseConstructor(args);
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConstructor<T>.UseAutoConstructor()
        {
            _constructionCommand = f => f.UseAutoConstructor();
            return this;
        }

        #endregion

        #region Passthrough configuration methods

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.Instantiate<TProp>(Expression<Func<T, TProp>> expr)
        {
            _configurationCommands.Add(f => f.Instantiate(expr));
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.Instantiate<TProp>(Expression<Func<T, TProp>> expr, Func<IConstructor<TProp>, TProp> func)
        {
            _configurationCommands.Add(f => f.Instantiate(expr, func));
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.Invoke(Expression<Action<T>> expr)
        {
            _configurationCommands.Add(f => f.Invoke(expr));
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.InvokePrivate(string methodName, params object[] arguments)
        {
            _configurationCommands.Add(f => f.InvokePrivate(methodName, arguments));
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.InvokePrivate<TProp>(Expression<Func<T, TProp>> expr, string methodName, params object[] arguments)
        {
            _configurationCommands.Add(f => f.InvokePrivate(expr, methodName, arguments));
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.With<TProp>(Expression<Func<T, TProp>> expr, TProp value)
        {
            _configurationCommands.Add(f => f.With(expr, value));
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value)
        {
            _configurationCommands.Add(f => f.WithBackingField(expr, value));
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value, string fieldName)
        {
            _configurationCommands.Add(f => f.WithBackingField(expr, value, fieldName));
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value)
        {
            _configurationCommands.Add(f => f.WithBackingFieldUntyped(expr, value));
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value, string fieldName)
        {
            _configurationCommands.Add(f => f.WithBackingFieldUntyped(expr, value, fieldName));
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithField<TValue>(string fieldName, TValue value)
        {
            _configurationCommands.Add(f => f.WithField(fieldName, value));
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithField<TProp, TValue>(Expression<Func<T, TProp>> expr, string fieldName, TValue value)
        {
            _configurationCommands.Add(f => f.WithField(expr, fieldName, value));
            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithSetter<TProp>(Expression<Func<T, TProp>> expr, TProp value)
        {
            _configurationCommands.Add(f => f.WithSetter(expr, value));
            return this;
        }

        #endregion
    }
}
