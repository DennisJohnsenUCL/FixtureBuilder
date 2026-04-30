using Bogus;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Bogus
{
    public interface IBogusConstructor<TReturn>
    {
        TReturn UseConstructor(Func<Faker, object[]> args);

        TReturn CreateUninitialized();

        TReturn CreateUninitialized(InitializeMembers initializeMembers);

        TReturn UseConstructor(params object[] args);

        TReturn UseAutoConstructor();

        TReturn UseCustomInstantiator(Func<Faker, TReturn> factory);
    }
}
