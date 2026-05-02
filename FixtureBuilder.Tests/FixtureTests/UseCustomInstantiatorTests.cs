namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class UseCustomInstantiatorTests
    {
        private class SimpleClass
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        private class ClassWithParameterizedConstructor(string name, int age)
        {
            public string Name { get; } = name;
            public int Age { get; } = age;
        }

        [Test]
        public void CreatesInstanceFromFactory()
        {
            var fixture = Fixture.New<SimpleClass>()
                .UseCustomInstantiator(() => new SimpleClass());

            var result = TestHelper.GetFixture(fixture);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void FactoryValues_ArePreserved()
        {
            var fixture = Fixture.New<ClassWithParameterizedConstructor>()
                .UseCustomInstantiator(() => new ClassWithParameterizedConstructor("Alice", 30));

            var result = TestHelper.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("Alice"));
                Assert.That(result.Age, Is.EqualTo(30));
            }
        }

        [Test]
        public void FollowedByConfiguration_AppliesBoth()
        {
            var result = Fixture.New<SimpleClass>()
                .UseCustomInstantiator(() => new SimpleClass { Name = "Alice" })
                .With(x => x.Value, 42)
                .Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("Alice"));
                Assert.That(result.Value, Is.EqualTo(42));
            }
        }

        [Test]
        public void Configuration_OverridesFactoryValue()
        {
            var result = Fixture.New<SimpleClass>()
                .UseCustomInstantiator(() => new SimpleClass { Name = "Alice" })
                .With(x => x.Name, "Bob")
                .Build();

            Assert.That(result.Name, Is.EqualTo("Bob"));
        }

        [Test]
        public void SubsequentBuild_ReturnsSameInstance()
        {
            var fixture = Fixture.New<SimpleClass>()
                .UseCustomInstantiator(() => new SimpleClass());

            var first = fixture.Build();
            var second = fixture.Build();

            Assert.That(first, Is.SameAs(second));
        }
    }
}
