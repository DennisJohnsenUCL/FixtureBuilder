using Bogus;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Bogus
{
    internal class BogusMemberInstantiator<T> : IBogusConstructor<T>
    {
        private readonly IConstructor<T> _constructor;
        private readonly Faker _faker;

        public BogusMemberInstantiator(IConstructor<T> constructor, Faker faker)
        {
            ArgumentNullException.ThrowIfNull(constructor);
            ArgumentNullException.ThrowIfNull(faker);
            _constructor = constructor;
            _faker = faker;
        }

        #region Faker methods

        public T UseConstructor(Func<Faker, object[]> args)
            => _constructor.UseConstructor(args(_faker));

        public T UseCustomInstantiator(Func<Faker, T> factory)
            => factory(_faker);

        #endregion

        #region Passthrough methods

        public T CreateUninitialized()
            => _constructor.CreateUninitialized();

        public T CreateUninitialized(InitializeMembers initializeMembers)
            => _constructor.CreateUninitialized(initializeMembers);

        public T UseAutoConstructor()
            => _constructor.UseAutoConstructor();

        public T UseConstructor(params object[] args)
            => _constructor.UseConstructor(args);

        public T UseCustomInstantiator(Func<T> instantiator)
            => _constructor.UseCustomInstantiator(instantiator);

        #endregion
    }
}
