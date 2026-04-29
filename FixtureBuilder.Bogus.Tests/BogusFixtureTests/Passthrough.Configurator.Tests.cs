namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    internal class PassthroughConfiguratorTests
    {
        private class SimpleClass
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        [Test]
        public void With_SetsPropertyValue()
        {
            var result = Fixture.WithBogus<SimpleClass>()
                .With(x => x.Name, "Alice")
                .Build();

            Assert.That(result.Name, Is.EqualTo("Alice"));
        }

        [Test]
        public void WithSetter_SetsPropertyValue()
        {
            var result = Fixture.WithBogus<SimpleClass>()
                .WithSetter(x => x.Name, "Bob")
                .Build();

            Assert.That(result.Name, Is.EqualTo("Bob"));
        }

        private class ClassWithField
        {
            public string _name = string.Empty;
            public string Name => _name;
        }
        [Test]
        public void WithField_SetsFieldValue()
        {
            var result = Fixture.WithBogus<ClassWithField>()
                .WithField("_name", "Charlie")
                .Build();

            Assert.That(result.Name, Is.EqualTo("Charlie"));
        }

        private class ClassWithBackingField
        {
            private readonly string _name = string.Empty;
            public string Name => _name;
        }
        [Test]
        public void WithBackingField_SetsBackingFieldValue()
        {
            var result = Fixture.WithBogus<ClassWithBackingField>()
                .WithBackingField(x => x.Name, "Dana")
                .Build();

            Assert.That(result.Name, Is.EqualTo("Dana"));
        }

        private class ClassWithMethod
        {
            public bool WasCalled { get; private set; }
            public void DoSomething() => WasCalled = true;
        }
        [Test]
        public void Invoke_ExecutesMethod()
        {
            var result = Fixture.WithBogus<ClassWithMethod>()
                .Invoke(x => x.DoSomething())
                .Build();

            Assert.That(result.WasCalled, Is.True);
        }

        [Test]
        public void MultipleCommands_AllApplied()
        {
            var result = Fixture.WithBogus<SimpleClass>()
                .With(x => x.Name, "Eve")
                .With(x => x.Value, 42)
                .Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.EqualTo("Eve"));
                Assert.That(result.Value, Is.EqualTo(42));
            }
        }
    }
}
