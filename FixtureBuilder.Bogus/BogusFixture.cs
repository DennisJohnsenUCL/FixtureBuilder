using System.Linq.Expressions;
using Bogus;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Bogus
{
    internal class BogusFixture<T> : IBogusFixtureConstructor<T>, IBogusFixtureConfigurator<T> where T : class
    {
        private readonly Faker _faker;

        private Func<IFixtureConstructor<T>, IFixtureConfigurator<T>>? _constructionCommand = null;
        private readonly List<Action<IFixtureConfigurator<T>>> _configurationCommands = [];

        public Randomizer Random { get => _faker.Random; set { _faker.Random = value; } }
        public string Locale { get => _faker.Locale; set { _faker.Locale = value; } }

        public BogusFixture(Faker faker)
        {
            ArgumentNullException.ThrowIfNull(faker);
            _faker = faker;
        }

        T IBogusFixtureConfigurator<T>.Build()
        {
            return ((IBogusFixtureConfigurator<T>)this).Build(1).First();
        }

        IEnumerable<T> IBogusFixtureConfigurator<T>.Build(int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), count, "Can only build positive numbers of objects.");

            var fixtures = new List<T>();
            for (int i = 0; i < count; i++)
            {
                var fixtureConstructor = Fixture.New<T>();

                IFixtureConfigurator<T> fixture = _constructionCommand != null
                    ? _constructionCommand(fixtureConstructor)
                    : fixtureConstructor;

                foreach (var command in _configurationCommands)
                {
                    command(fixture);
                }
                fixtures.Add(fixture.Build());
            }

            return fixtures;
        }

        private BogusFixture<T> AddConstruction(Func<IFixtureConstructor<T>, IFixtureConfigurator<T>> func)
        {
            _constructionCommand = func;
            return this;
        }

        private BogusFixture<T> AddConfiguration(Action<IFixtureConfigurator<T>> action)
        {
            _configurationCommands.Add(action);
            return this;
        }

        #region Faker construction methods

        IBogusFixtureConfigurator<T> IBogusFixtureConstructor<T>.UseConstructor(Func<Faker, object[]> args)
            => AddConstruction(f => f.UseConstructor(args(_faker)));

        IBogusFixtureConfigurator<T> IBogusFixtureConstructor<T>.UseCustomInstantiator(Func<Faker, T> factory)
            => AddConstruction(_ => Fixture.New(factory(_faker)));

        #endregion

        #region Faker configuration methods

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.Instantiate<TProp>(Expression<Func<T, TProp>> expr, Func<IBogusConstructor<TProp>, TProp> func)
        {
            TProp instantiateFunc(IConstructor<TProp> constructor)
            {
                var bogusConstructor = new BogusMemberInstantiator<TProp>(constructor, _faker);
                return func(bogusConstructor);
            }

            _configurationCommands.Add(fixture => fixture.Instantiate(expr, instantiateFunc));

            return this;
        }

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithField<TValue>(string fieldName, Func<Faker, TValue> value)
            => AddConfiguration(f => f.WithField(fieldName, value(_faker)));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithField<TProp, TValue>(Expression<Func<T, TProp>> expr, string fieldName, Func<Faker, TValue> value)
            => AddConfiguration(f => f.WithField(expr, fieldName, value(_faker)));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithBackingField<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, TProp> value)
            => AddConfiguration(f => f.WithBackingField(expr, value(_faker)));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithBackingField<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, TProp> value, string fieldName)
            => AddConfiguration(f => f.WithBackingField(expr, value(_faker), fieldName));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, object?> value)
            => AddConfiguration(f => f.WithBackingFieldUntyped(expr, value(_faker)));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, object?> value, string fieldName)
            => AddConfiguration(f => f.WithBackingFieldUntyped(expr, value(_faker), fieldName));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithSetter<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, TProp> value)
            => AddConfiguration(f => f.WithSetter(expr, value(_faker)));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.With<TProp>(Expression<Func<T, TProp>> expr, Func<Faker, TProp> value)
            => AddConfiguration(f => f.With(expr, value(_faker)));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.Invoke(Func<Faker, Expression<Action<T>>> expr)
            => AddConfiguration(f => f.Invoke(expr(_faker)));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.InvokePrivate(string methodName, Func<Faker, object[]> arguments)
            => AddConfiguration(f => f.InvokePrivate(methodName, arguments(_faker)));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.InvokePrivate<TProp>(Expression<Func<T, TProp>> expr, string methodName, Func<Faker, object[]> arguments)
            => AddConfiguration(f => f.InvokePrivate(expr, methodName, arguments(_faker)));

        #endregion

        #region Passthrough construction methods

        IBogusFixtureConfigurator<T> IBogusFixtureConstructor<T>.CreateUninitialized()
            => AddConstruction(f => f.CreateUninitialized());

        IBogusFixtureConfigurator<T> IBogusFixtureConstructor<T>.CreateUninitialized(InitializeMembers initializeMembers)
            => AddConstruction(f => f.CreateUninitialized(initializeMembers));

        IBogusFixtureConfigurator<T> IBogusFixtureConstructor<T>.UseConstructor(params object[] args)
            => AddConstruction(f => f.UseConstructor(args));

        IBogusFixtureConfigurator<T> IBogusFixtureConstructor<T>.UseAutoConstructor()
            => AddConstruction(f => f.UseAutoConstructor());

        #endregion

        #region Passthrough configuration methods

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.Instantiate<TProp>(Expression<Func<T, TProp>> expr)
            => AddConfiguration(f => f.Instantiate(expr));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.Invoke(Expression<Action<T>> expr)
            => AddConfiguration(f => f.Invoke(expr));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.InvokePrivate(string methodName, params object[] arguments)
            => AddConfiguration(f => f.InvokePrivate(methodName, arguments));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.InvokePrivate<TProp>(Expression<Func<T, TProp>> expr, string methodName, params object[] arguments)
            => AddConfiguration(f => f.InvokePrivate(expr, methodName, arguments));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.With<TProp>(Expression<Func<T, TProp>> expr, TProp value)
            => AddConfiguration(f => f.With(expr, value));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value)
            => AddConfiguration(f => f.WithBackingField(expr, value));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithBackingField<TProp>(Expression<Func<T, TProp>> expr, TProp value, string fieldName)
            => AddConfiguration(f => f.WithBackingField(expr, value, fieldName));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value)
            => AddConfiguration(f => f.WithBackingFieldUntyped(expr, value));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithBackingFieldUntyped<TProp>(Expression<Func<T, TProp>> expr, object? value, string fieldName)
            => AddConfiguration(f => f.WithBackingFieldUntyped(expr, value, fieldName));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithField<TValue>(string fieldName, TValue value)
            => AddConfiguration(f => f.WithField(fieldName, value));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithField<TProp, TValue>(Expression<Func<T, TProp>> expr, string fieldName, TValue value)
            => AddConfiguration(f => f.WithField(expr, fieldName, value));

        IBogusFixtureConfigurator<T> IBogusFixtureConfigurator<T>.WithSetter<TProp>(Expression<Func<T, TProp>> expr, TProp value)
            => AddConfiguration(f => f.WithSetter(expr, value));

        #endregion
    }
}
