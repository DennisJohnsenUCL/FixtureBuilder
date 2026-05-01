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

        public T CreateUninitialized()
        {
            return _constructor.CreateUninitialized();
        }

        public T CreateUninitialized(InitializeMembers initializeMembers)
        {
            return _constructor.CreateUninitialized(initializeMembers);
        }

        public T UseAutoConstructor()
        {
            return _constructor.UseAutoConstructor();
        }

        public T UseConstructor(Func<Faker, object[]> args)
        {
            return _constructor.UseConstructor(args(_faker));
        }

        public T UseConstructor(params object[] args)
        {
            return _constructor.UseConstructor(args);
        }

        public T UseCustomInstantiator(Func<Faker, T> factory)
        {
            return factory(_faker);
        }
    }
}
