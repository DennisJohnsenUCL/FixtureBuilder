namespace FixtureBuilder.Tests
{
    internal class NewTests
    {
        [Test]
        public void ReturnsIFixtureConstructor()
        {
            var method = typeof(Fixture).GetMethod("New", []);

            Assert.That(method!.ReturnType.GetGenericTypeDefinition, Is.EqualTo(typeof(IFixtureConstructor<>)));
        }

        class ClassWithString
        {
            public string Text = null!;
        }
        [Test]
        public void PreBuiltFixture_SetsFixture()
        {
            var text = "test string";

            var preBuiltFixture = new ClassWithString() { Text = text };

            var fixture = Fixture.New(preBuiltFixture);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(text));
        }

        [Test]
        public void GenericTypeInterface_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<INestedInterface>());
        }
    }
}
