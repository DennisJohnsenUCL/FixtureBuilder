using FixtureBuilder.Core;

namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class ImplicitConstructionTests
    {
        [Test]
        public void DefaultOptions_ConstructsWithAutoConstructor()
        {
            var fixture = Fixture.New<ConstructorSetsValueClass>();

            var result = fixture.Build();

            Assert.That(result.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void CreateUninitializedOption_CreatesUninitializedWithDefaultInitializeMembers()
        {
            var options = new FixtureOptions
            {
                DefaultFixtureInstantiationMethod = InstantiationMethod.CreateUninitialized,
                DefaultInitializeMembers = InitializeMembers.All
            };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<ConstructorSetsValueClass>();

            var result = fixture.Build();

            Assert.That(result.Value, Is.EqualTo("Value"));
        }

        [Test]
        public void CreateUninitializedOption_TypeThatCannotBeCreatedUninitialized_ThrowsInvalidOperationException()
        {
            var options = new FixtureOptions { DefaultFixtureInstantiationMethod = InstantiationMethod.CreateUninitialized };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<string>();

            Assert.Throws<InvalidOperationException>(() => fixture.Build());
        }

        [Test]
        public void DefaultConstructorOption_ConstructsWithParameterlessConstructor()
        {
            var options = new FixtureOptions { DefaultFixtureInstantiationMethod = InstantiationMethod.UseDefaultConstructor };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<ConstructorSetsValueClass>();

            var result = fixture.Build();

            Assert.That(result.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void DefaultConstructorOption_NoParameterlessConstructor_ThrowsInvalidOperationException()
        {
            var options = new FixtureOptions { DefaultFixtureInstantiationMethod = InstantiationMethod.UseDefaultConstructor };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<StringConstructorClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.Build());
        }

        [Test]
        public void DefaultInstantiationMethodOption_AppliesToFixture()
        {
            var options = new FixtureOptions { DefaultInstantiationMethod = InstantiationMethod.CreateUninitialized };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<ConstructorSetsValueClass>();

            var result = fixture.Build();

            Assert.That(result.Value, Is.Null);
        }

        [Test]
        public void BuildCalledTwice_ReturnsSameInstance()
        {
            var fixture = Fixture.New<ConstructorSetsValueClass>();

            var first = fixture.Build();
            var second = fixture.Build();

            Assert.That(second, Is.SameAs(first));
        }

        [Test]
        public void NotAllowed_Build_ThrowsInvalidOperationException()
        {
            var options = new FixtureOptions { AllowImplicitConstruction = false };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<ConstructorSetsValueClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.Build());
        }

        [Test]
        public void NotAllowed_ConfigurationMethod_ThrowsInvalidOperationException()
        {
            var options = new FixtureOptions { AllowImplicitConstruction = false };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<ConstructorSetsValueClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.With(x => x.Value, "value"));
        }

        [Test]
        public void NotAllowed_ConstructedExplicitly_ConfigurationMethodSetsValue()
        {
            var options = new FixtureOptions { AllowImplicitConstruction = false };
            var factory = new FixtureFactory(options);
            var fixture = factory.New<ConstructorSetsValueClass>();

            var result = fixture.UseConstructor().With(x => x.Value, "value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }
    }
}
