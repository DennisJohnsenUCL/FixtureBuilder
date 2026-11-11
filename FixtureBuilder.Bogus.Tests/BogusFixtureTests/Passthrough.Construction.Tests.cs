namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class PassthroughConstructionTests
    {
        [Test]
        public void UseAutoConstructor_ConstructsWithAutoConstructor()
        {
            var fixture = Fixture.WithBogus<ConstructorSetsValueClass>();

            var result = fixture.UseAutoConstructor().Build();

            Assert.That(result.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void UseConstructor_Arguments_UsesMatchingConstructor()
        {
            var fixture = Fixture.WithBogus<StringConstructorClass>();

            var result = fixture.UseConstructor("provided").Build();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        [Test]
        public void CreateUninitialized_BypassesConstructor()
        {
            var fixture = Fixture.WithBogus<ConstructorSetsValueClass>();

            var result = fixture.CreateUninitialized().Build();

            Assert.That(result.Value, Is.Null);
        }

        [Test]
        public void CreateUninitialized_InitializeMembers_InitializesMembers()
        {
            var fixture = Fixture.WithBogus<PropertyHolder<string>>();

            var result = fixture.CreateUninitialized(InitializeMembers.All).Build();

            Assert.That(result.Value, Is.EqualTo("Value"));
        }

        [Test]
        public void UseCustomInstantiator_UsesCreatedInstance()
        {
            var fixture = Fixture.WithBogus<StringConstructorClass>();

            var result = fixture.UseCustomInstantiator(() => new StringConstructorClass("provided")).Build();

            Assert.That(result.Value, Is.EqualTo("provided"));
        }

        private class TwoPropertiesClass
        {
            public string First { get; set; } = null!;
            public string Second { get; set; } = null!;
        }

        [Test]
        public void ConstructionThenConfiguration_AppliesBoth()
        {
            var fixture = Fixture.WithBogus<TwoPropertiesClass>();

            var result = fixture.UseCustomInstantiator(() => new TwoPropertiesClass { First = "first" }).With(x => x.Second, "second").Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.First, Is.EqualTo("first"));
                Assert.That(result.Second, Is.EqualTo("second"));
            }
        }
    }
}
