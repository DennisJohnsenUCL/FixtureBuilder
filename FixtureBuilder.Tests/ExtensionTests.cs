using FixtureBuilder.Extensions;

namespace FixtureBuilder.Tests
{
    internal sealed class ExtensionTests
    {
        private readonly static string _text = "Test string";

        [Test]
        public void With_Property_SetsProperty()
        {
            var fixture = Fixture.New<TestClass>().Build();

            fixture = fixture.With(f => f.Text, _text);

            Assert.That(fixture.Text, Is.EqualTo(_text));
        }

        [Test]
        public void WithField_Property_SetsProperty()
        {
            var fixture = Fixture.New<TestClass>().Build();

            fixture = fixture.WithField(f => f.Text, _text);

            Assert.That(fixture.Text, Is.EqualTo(_text));
        }

        [Test]
        public void WithField_FieldName_SetsField()
        {
            var fieldName = "_privateExplicitField";

            var fixture = Fixture.New<TestClass>().BypassConstructor().WithField(fieldName, _text).Build();

            Assert.That(fixture.PrivateExplicitField, Is.EqualTo(_text));
        }

        [Test]
        public void WithField_FieldName_CollectionTypeField_SetsField()
        {
            var fieldName = "_stringListField";
            var secondEntry = "More test";

            var fixture = Fixture.New<TestClass>().Build();

            fixture = fixture.WithField(fieldName, [_text, secondEntry]);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(fixture.StringListProp[0], Is.EqualTo(_text));
                Assert.That(fixture.StringListProp[1], Is.EqualTo(secondEntry));
            }
        }

        [Test]
        public void WithField_FieldNameAndProperty_SetsProperty()
        {
            var fieldName = "_privateField";

            var fixture = Fixture.New<TestClass>().Build();

            fixture = fixture.WithField(fieldName, t => t.NestedClass.DeeperNestedClass.PrivateFieldGetter, _text);

            Assert.That(fixture.NestedClass.DeeperNestedClass.PrivateFieldGetter, Is.EqualTo(_text));
        }

        [Test]
        public void WithSetter_Property_SetsProperty()
        {
            var fixture = Fixture.New<TestClass>().Build();

            fixture = fixture.WithSetter(t => t.Text, _text);

            Assert.That(fixture.Text, Is.EqualTo(_text));
        }
    }
}
