using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests
{
    internal sealed class ExtensionTests
    {
        private readonly static string _testString = "Test string";

        class TestClass
        {
            private readonly string _text = null!;
            public string Text => _text;
            public string OtherText { get; set; } = null!;
        }

        [Test]
        public void With_Property_SetsProperty()
        {
            var fixture = new TestClass();

            fixture = fixture.With(f => f.Text, _testString);

            Assert.That(fixture.Text, Is.EqualTo(_testString));
        }

        [Test]
        public void WithField_Property_SetsProperty()
        {
            var fixture = new TestClass();

            fixture = fixture.WithField(f => f.OtherText, _testString);

            Assert.That(fixture.OtherText, Is.EqualTo(_testString));
        }

        [Test]
        public void WithField_FieldName_SetsField()
        {
            var fieldName = "_text";

            var fixture = new TestClass();

            fixture = fixture.WithField(fieldName, _testString);

            Assert.That(fixture.Text, Is.EqualTo(_testString));
        }

        [Test]
        public void WithField_FieldNameAndProperty_SetsProperty()
        {
            var fieldName = "_text";

            var fixture = new TestClass();

            fixture = fixture.WithField(fieldName, t => t.Text, _testString);

            Assert.That(fixture.Text, Is.EqualTo(_testString));
        }

        [Test]
        public void WithSetter_Property_SetsProperty()
        {
            var fixture = new TestClass();

            fixture = fixture.WithSetter(t => t.OtherText, _testString);

            Assert.That(fixture.OtherText, Is.EqualTo(_testString));
        }
    }
}
