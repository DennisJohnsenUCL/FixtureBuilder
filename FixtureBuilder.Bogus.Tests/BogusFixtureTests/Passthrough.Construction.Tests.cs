using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal sealed class PassthroughConstructionTests
    {
        private class SimpleClass
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        private class ClassWithParameterizedConstructor(string name, int value)
        {
            public string Name { get; } = name;
            public int Value { get; } = value;
        }

        [Test]
        public void UseAutoConstructor_ReturnsInstance()
        {
            var result = Fixture.WithBogus<SimpleClass>()
                .UseAutoConstructor()
                .Build();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void UseConstructor_WithArgs_SetsValues()
        {
            var result = Fixture.WithBogus<ClassWithParameterizedConstructor>()
                .UseConstructor("Alice", 42)
                .Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("Alice"));
                Assert.That(result.Value, Is.EqualTo(42));
            }
        }

        [Test]
        public void CreateUninitialized_ReturnsInstance()
        {
            var result = Fixture.WithBogus<SimpleClass>()
                .CreateUninitialized()
                .Build();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CreateUninitialized_WithInitializeMembers_ReturnsInstance()
        {
            var result = Fixture.WithBogus<SimpleClass>()
                .CreateUninitialized(InitializeMembers.None)
                .Build();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void UseAutoConstructor_FollowedByConfiguration_AppliesBoth()
        {
            var result = Fixture.WithBogus<SimpleClass>()
                .UseAutoConstructor()
                .With(x => x.Name, "Bob")
                .Build();

            Assert.That(result.Name, Is.EqualTo("Bob"));
        }

        [Test]
        public void UseCustomInstantiator_ReturnsInstance()
        {
            var result = Fixture.WithBogus<SimpleClass>()
                .UseCustomInstantiator(() => new SimpleClass { Name = "Custom" })
                .Build();

            Assert.That(result.Name, Is.EqualTo("Custom"));
        }

        [Test]
        public void UseCustomInstantiator_FollowedByConfiguration_AppliesBoth()
        {
            var result = Fixture.WithBogus<SimpleClass>()
                .UseCustomInstantiator(() => new SimpleClass { Name = "Custom" })
                .With(x => x.Value, 99)
                .Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("Custom"));
                Assert.That(result.Value, Is.EqualTo(99));
            }
        }
    }
}
