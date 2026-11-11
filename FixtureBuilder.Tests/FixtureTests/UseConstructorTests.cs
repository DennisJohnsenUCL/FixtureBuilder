namespace FixtureBuilder.Tests.FixtureTests
{
    internal sealed class UseConstructorTests
    {
        [Test]
        public void NoArguments_UsesParameterlessConstructor()
        {
            var fixture = Fixture.New<ConstructorSetsValueClass>();

            var result = fixture.UseConstructor().Build();

            Assert.That(result.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void NoArguments_PrivateParameterlessConstructor_UsesIt()
        {
            var fixture = Fixture.New<PrivateConstructorClass>();

            var result = fixture.UseConstructor().Build();

            Assert.That(result.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void Arguments_UsesMatchingConstructor()
        {
            var fixture = Fixture.New<StringConstructorClass>();

            var result = fixture.UseConstructor("value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        private class NullableTextAndNumberClass(string? text, int number)
        {
            public string? Text { get; } = text;
            public int Number { get; } = number;
        }

        [Test]
        public void Arguments_NullArgument_UsesMatchingConstructor()
        {
            var fixture = Fixture.New<NullableTextAndNumberClass>();

            var result = fixture.UseConstructor(null!, 7).Build();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Text, Is.Null);
                Assert.That(result.Number, Is.EqualTo(7));
            }
        }

        [Test]
        public void NoArguments_NoParameterlessConstructor_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<StringConstructorClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.UseConstructor());
        }

        [Test]
        public void ArgumentsMatchNoConstructor_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<StringConstructorClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.UseConstructor("value", 123));
        }

        private record InitializedPropertyRecord
        {
            public string Value { get; init; } = "constructor";
        }

        [Test]
        public void Record_NoArguments_UsesParameterlessConstructorOverCopyConstructor()
        {
            var fixture = Fixture.New<InitializedPropertyRecord>();

            var result = fixture.UseConstructor().Build();

            Assert.That(result.Value, Is.EqualTo("constructor"));
        }

        [Test]
        public void PositionalRecord_Arguments_UsesPrimaryConstructor()
        {
            var fixture = Fixture.New<PositionalRecord>();

            var result = fixture.UseConstructor("value").Build();

            Assert.That(result.Value, Is.EqualTo("value"));
        }

        [Test]
        public void AlreadyConstructed_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<ConstructorSetsValueClass>();
            fixture.UseConstructor();

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseConstructor());

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }

        [Test]
        public void AfterConfigurationMethod_ThrowsInvalidOperationException()
        {
            var fixture = Fixture.New<ConstructorSetsValueClass>();
            fixture.With(x => x.Value, "value");

            var ex = Assert.Throws<InvalidOperationException>(() => fixture.UseConstructor());

            Assert.That(ex.Message, Does.Contain("already constructed"));
        }
    }
}
