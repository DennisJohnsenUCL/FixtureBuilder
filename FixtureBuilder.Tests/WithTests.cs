namespace FixtureBuilder.Tests
{
    internal sealed class WithTests
    {
        class PropWithSetterClass
        {
            public string Text { get; set; } = null!;
        }
        [Test]
        public void PropWithSetter_SetsProperty()
        {
            var text = "Test string";

            var fixture = Fixture.New<PropWithSetterClass>().With(t => t.Text, text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(text));
        }

        class PropWithSetterAndUnrecognizedFieldClass
        {
            private string _unrelatedFieldName = null!;
            public string Text { get => _unrelatedFieldName; set { _unrelatedFieldName = value; } }
        }
        [Test]
        public void PropWithSetterAndUnrecognizedField_SetsProperty()
        {
            var text = "Test string";

            var fixture = Fixture.New<PropWithSetterAndUnrecognizedFieldClass>().With(t => t.Text, text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(text));
        }

        class PropWithoutSetterClass
        {
            private readonly string _text = null!;
            public string Text => _text;
        }
        [Test]
        public void PropWithoutSetter_SetsProperty()
        {
            var text = "Test string";

            var fixture = Fixture.New<PropWithoutSetterClass>().With(t => t.Text, text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(text));
        }

        class PropWithoutSetterAndUnrecognizedFieldClass
        {
            private readonly string _unrelatedFieldName = null!;
            public string Text => _unrelatedFieldName;
        }
        [Test]
        public void PropWithoutSetterAndUnrecognizedField_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => Fixture.New<PropWithoutSetterAndUnrecognizedFieldClass>().With(t => t.Text, "Test"));
        }
    }
}
