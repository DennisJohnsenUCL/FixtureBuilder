namespace FixtureBuilder.Tests.FixtureTests
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

            var fixture = TestHelper.MakeFixture<PropWithSetterClass>();

            fixture.With(t => t.Text, text);

            var field = TestHelper.GetFixture(fixture);
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

            var fixture = TestHelper.MakeFixture<PropWithSetterAndUnrecognizedFieldClass>();

            fixture.With(t => t.Text, text);

            var field = TestHelper.GetFixture(fixture);
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

            var fixture = TestHelper.MakeFixture<PropWithoutSetterClass>();

            fixture.With(t => t.Text, text);

            var field = TestHelper.GetFixture(fixture);
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
            var fixture = TestHelper.MakeFixture<PropWithoutSetterAndUnrecognizedFieldClass>();

            Assert.Throws<InvalidOperationException>(() => fixture.With(t => t.Text, "Test"));
        }
    }
}
