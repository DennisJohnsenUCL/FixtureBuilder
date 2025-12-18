namespace FixtureBuilder.Tests
{
    internal sealed partial class WithFieldTests
    {
        [Test]
        public void ClassPrivateField_SetsField()
        {
            var fieldName = "_text";

            var fixture = Fixture.New<ExplicitBackingFieldClass>().BypassConstructor().WithField(fieldName, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(_text));
        }

        [Test]
        public void IncorrectFieldName_ThrowsException()
        {
            var fieldName = "_notAField";

            Assert.Throws<InvalidOperationException>(() => Fixture.New<ExplicitBackingFieldClass>().BypassConstructor().WithField(fieldName, _text));
        }

        [Test]
        public void IncorrectFieldType_ThrowsException()
        {
            var fieldName = "_text";

            Assert.Throws<InvalidOperationException>(() => Fixture.New<ExplicitBackingFieldClass>().BypassConstructor().WithField(fieldName, _number));
        }

        [Test]
        public void InheritedProtectedField_SetsField()
        {
            var fieldName = "_inheritedField";

            var fixture = Fixture.New<DerivedTestClass>().BypassConstructor().WithField(fieldName, _text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.InheritedFieldGetter, Is.EqualTo(_text));
        }

        record FieldRecord
        {
            private readonly string _text = null!;
            public string Text => _text;
        }
        [Test]
        public void RecordField_SetsField()
        {
            var text = "Test string";

            var fixture = Fixture.New<FieldRecord>().WithField("_text", text);
            var field = Helpers.GetFixture(fixture);

            Assert.That(field.Text, Is.EqualTo(text));
        }

        [Test]
        public void Field_ConstructionNotChosen_InstantiatesNonNullables()
        {
            var fixture = Fixture.New<ClassWithNullable>().WithField("_text", "test");
            var field = Helpers.GetFixture(fixture);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(field.NullableClass, Is.Null);
                Assert.That(field.NonNullableClass, Is.Not.Null);
            }
        }
    }
}
