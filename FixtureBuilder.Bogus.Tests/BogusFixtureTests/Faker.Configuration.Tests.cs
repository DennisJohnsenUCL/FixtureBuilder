namespace FixtureBuilder.Bogus.Tests.BogusFixtureTests
{
    public class FakerConfigurationTests
    {
        private class SimpleClass
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        private class ClassWithField
        {
            public string _name = string.Empty;
            public string Name => _name;
        }

        private class ClassWithBackingField
        {
            private readonly string _name = string.Empty;
            public string Name => _name;
        }

        private class ClassWithMethod
        {
            public string LastArg { get; private set; } = string.Empty;
            public void SetArg(string arg) => LastArg = arg;
        }

        [Test]
        public void With_Faker_SetsPropertyValue()
        {
            var result = Fixture.WithBogus<SimpleClass>()
                .With(x => x.Name, f => f.Name.FirstName())
                .Build();

            Assert.That(result.Name, Is.Not.Empty);
        }

        [Test]
        public void With_Faker_BuildMultiple_ProducesDifferentValues()
        {
            var results = Fixture.WithBogus<SimpleClass>()
                .With(x => x.Name, f => f.Name.FirstName())
                .Build(2)
                .ToList();

            Assert.That(results[0].Name, Is.Not.EqualTo(results[1].Name));
        }

        [Test]
        public void WithSetter_Faker_SetsPropertyValue()
        {
            var result = Fixture.WithBogus<SimpleClass>()
                .WithSetter(x => x.Name, f => f.Name.LastName())
                .Build();

            Assert.That(result.Name, Is.Not.Empty);
        }

        [Test]
        public void WithField_Faker_SetsFieldValue()
        {
            var result = Fixture.WithBogus<ClassWithField>()
                .WithField("_name", f => f.Name.FirstName())
                .Build();

            Assert.That(result.Name, Is.Not.Empty);
        }

        [Test]
        public void WithBackingField_Faker_SetsBackingFieldValue()
        {
            var result = Fixture.WithBogus<ClassWithBackingField>()
                .WithBackingField(x => x.Name, f => f.Name.FirstName())
                .Build();

            Assert.That(result.Name, Is.Not.Empty);
        }

        [Test]
        public void InvokePrivate_Faker_PassesGeneratedArgs()
        {
            var result = Fixture.WithBogus<ClassWithMethod>()
                .InvokePrivate("SetArg", f => [f.Name.FirstName()])
                .Build();

            Assert.That(result.LastArg, Is.Not.Empty);
        }

        [Test]
        public void MultipleFakerCommands_AllApplied()
        {
            var result = Fixture.WithBogus<SimpleClass>()
                .With(x => x.Name, f => f.Name.FirstName())
                .With(x => x.Value, f => f.Random.Int(1, 100))
                .Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.Not.Empty);
                Assert.That(result.Value, Is.InRange(1, 100));
            }
        }

        [Test]
        public void MixedFakerAndPassthrough_AllApplied()
        {
            var result = Fixture.WithBogus<SimpleClass>()
                .With(x => x.Name, f => f.Name.FirstName())
                .With(x => x.Value, 42)
                .Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Name, Is.Not.Empty);
                Assert.That(result.Value, Is.EqualTo(42));
            }
        }

        private class ClassWithNestedField
        {
            public ClassWithField Child { get; set; } = new();
        }

        private class ClassWithNestedMethod
        {
            public ClassWithMethod Child { get; set; } = new();
        }

        [Test]
        public void WithField_Faker_Nested_SetsFieldValue()
        {
            var result = Fixture.WithBogus<ClassWithNestedField>()
                .WithField<ClassWithField, string>(x => x.Child, "_name", f => f.Name.FirstName())
                .Build();

            Assert.That(result.Child.Name, Is.Not.Empty);
        }

        [Test]
        public void WithBackingField_Faker_WithFieldName_SetsValue()
        {
            var result = Fixture.WithBogus<ClassWithBackingField>()
                .WithBackingField(x => x.Name, f => f.Name.FirstName(), "_name")
                .Build();

            Assert.That(result.Name, Is.Not.Empty);
        }

        [Test]
        public void WithBackingFieldUntyped_Faker_SetsValue()
        {
            var result = Fixture.WithBogus<ClassWithBackingField>()
                .WithBackingFieldUntyped(x => x.Name, f => f.Name.FirstName())
                .Build();

            Assert.That(result.Name, Is.Not.Empty);
        }

        [Test]
        public void WithBackingFieldUntyped_Faker_WithFieldName_SetsValue()
        {
            var result = Fixture.WithBogus<ClassWithBackingField>()
                .WithBackingFieldUntyped(x => x.Name, f => f.Name.FirstName(), "_name")
                .Build();

            Assert.That(result.Name, Is.Not.Empty);
        }

        [Test]
        public void Invoke_Faker_ExecutesMethod()
        {
            var result = Fixture.WithBogus<ClassWithMethod>()
                .Invoke(f => x => x.SetArg(f.Name.FirstName()))
                .Build();

            Assert.That(result.LastArg, Is.Not.Empty);
        }

        [Test]
        public void InvokePrivate_Faker_Nested_PassesGeneratedArgs()
        {
            var result = Fixture.WithBogus<ClassWithNestedMethod>()
                .InvokePrivate(x => x.Child, "SetArg", f => [f.Name.FirstName()])
                .Build();

            Assert.That(result.Child.LastArg, Is.Not.Empty);
        }
    }
}
